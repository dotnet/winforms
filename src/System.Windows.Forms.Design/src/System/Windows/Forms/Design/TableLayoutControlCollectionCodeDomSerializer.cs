// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design
{
    //This is the serializer for TableLayoutControlCollection. It uses the Add(control, col, row)
    //syntax to add a control to the specific cell of the table whenever appropriate.
    //most of the code is copied from CollectionCodeDomSerializer. 
    internal class TableLayoutControlCollectionCodeDomSerializer : CollectionCodeDomSerializer
    {
        /// <summary>
        ///  Serializes the given collection.  targetExpression will refer to the expression used to rever to the 
        ///  collection, but it can be null.
        /// </summary>
        protected override object SerializeCollection(IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize)
        {
            // Here we need to invoke Add once for each and every item in the collection. We can re-use the property
            // reference and method reference, but we will need to recreate the invoke statement each time.
            CodeStatementCollection statements = new CodeStatementCollection();
            CodeMethodReferenceExpression methodRef = new CodeMethodReferenceExpression(targetExpression, "Add");
            TableLayoutControlCollection tableCollection = (TableLayoutControlCollection)originalCollection;

            if (valuesToSerialize.Count > 0)
            {
                bool isTargetInherited = false;
                ExpressionContext cxt = manager.Context[typeof(ExpressionContext)] as ExpressionContext;

                if (cxt != null && cxt.Expression == targetExpression)
                {
                    IComponent comp = cxt.Owner as IComponent;

                    if (comp != null)
                    {
                        InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
                        isTargetInherited = (ia != null && ia.InheritanceLevel == InheritanceLevel.Inherited);
                    }
                }

                foreach (object o in valuesToSerialize)
                {
                    bool genCode = !(o is IComponent);

                    if (!genCode)
                    {
                        InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)];

                        if (ia != null)
                        {
                            if (ia.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                                genCode = false;
                            else if (ia.InheritanceLevel == InheritanceLevel.Inherited && isTargetInherited)
                                genCode = false;
                            else
                                genCode = true;
                        }
                        else
                        {
                            genCode = true;
                        }
                    }

                    if (genCode)
                    {
                        CodeMethodInvokeExpression statement = new CodeMethodInvokeExpression();
                        statement.Method = methodRef;
                        CodeExpression serializedObj = SerializeToExpression(manager, o);

                        if (serializedObj != null && !typeof(Control).IsAssignableFrom(o.GetType()))
                        {
                            serializedObj = new CodeCastExpression(typeof(Control), serializedObj);
                        }

                        if (serializedObj != null)
                        {
                            int col, row;
                            col = tableCollection.Container.GetColumn((Control)o);
                            row = tableCollection.Container.GetRow((Control)o);
                            statement.Parameters.Add(serializedObj);

                            if (col != -1 || row != -1)
                            {
                                statement.Parameters.Add(new CodePrimitiveExpression(col));
                                statement.Parameters.Add(new CodePrimitiveExpression(row));
                            }

                            statements.Add(statement);
                        }
                    }
                }
            }

            return statements;
        }
    }
}
