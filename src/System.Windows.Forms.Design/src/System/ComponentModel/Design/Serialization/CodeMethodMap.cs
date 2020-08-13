// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This structure is used by IntegrateStatements to put statements in the right place.
    /// </summary>
    internal class CodeMethodMap
    {
        private CodeStatementCollection _container;
        private CodeStatementCollection _begin;
        private CodeStatementCollection _end;
        private CodeStatementCollection _statements;
        private CodeStatementCollection _locals;
        private CodeStatementCollection _fields;
        private CodeStatementCollection _variables;
        private readonly CodeStatementCollection _targetStatements;
        private readonly CodeMemberMethod _method;

        internal CodeMethodMap(CodeMemberMethod method) : this(null, method)
        {
        }

        internal CodeMethodMap(CodeStatementCollection targetStatements, CodeMemberMethod method)
        {
            _method = method;
            if (targetStatements != null)
            {
                _targetStatements = targetStatements;
            }
            else
            {
                _targetStatements = _method.Statements;
            }
        }

        internal CodeStatementCollection BeginStatements
        {
            get
            {
                if (_begin is null)
                {
                    _begin = new CodeStatementCollection();
                }

                return _begin;
            }
        }

        internal CodeStatementCollection EndStatements
        {
            get
            {
                if (_end is null)
                {
                    _end = new CodeStatementCollection();
                }

                return _end;
            }
        }

        internal CodeStatementCollection ContainerStatements
        {
            get
            {
                if (_container is null)
                {
                    _container = new CodeStatementCollection();
                }

                return _container;
            }
        }

        internal CodeMemberMethod Method
        {
            get => _method;
        }

        internal CodeStatementCollection Statements
        {
            get
            {
                if (_statements is null)
                {
                    _statements = new CodeStatementCollection();
                }

                return _statements;
            }
        }

        internal CodeStatementCollection LocalVariables
        {
            get
            {
                if (_locals is null)
                {
                    _locals = new CodeStatementCollection();
                }

                return _locals;
            }
        }

        internal CodeStatementCollection FieldAssignments
        {
            get
            {
                if (_fields is null)
                {
                    _fields = new CodeStatementCollection();
                }

                return _fields;
            }
        }

        //TODO: Should we update RootCodeDomSerializer as well?
        internal CodeStatementCollection VariableAssignments
        {
            get
            {
                if (_variables is null)
                {
                    _variables = new CodeStatementCollection();
                }

                return _variables;
            }
        }

        internal void Add(CodeStatementCollection statements)
        {
            foreach (CodeStatement statement in statements)
            {
                if (statement.UserData["IContainer"] is string isContainer && isContainer == "IContainer")
                {
                    ContainerStatements.Add(statement);
                }
                else if (statement is CodeAssignStatement && ((CodeAssignStatement)statement).Left is CodeFieldReferenceExpression)
                {
                    FieldAssignments.Add(statement);
                }
                else if (statement is CodeAssignStatement && ((CodeAssignStatement)statement).Left is CodeVariableReferenceExpression)
                {
                    VariableAssignments.Add(statement);
                }
                else if (statement is CodeVariableDeclarationStatement)
                {
                    LocalVariables.Add(statement);
                }
                else
                {
                    if (statement.UserData["statement-ordering"] is string order)
                    {
                        switch (order)
                        {
                            case "begin":
                                BeginStatements.Add(statement);
                                break;

                            case "end":
                                EndStatements.Add(statement);
                                break;

                            case "default":
                            default:
                                Statements.Add(statement);
                                break;
                        }
                    }
                    else
                    {
                        Statements.Add(statement);
                    }
                }
            }
        }

        internal void Combine()
        {
            if (_container != null)
            {
                _targetStatements.AddRange(_container);
            }

            if (_locals != null)
            {
                _targetStatements.AddRange(_locals);
            }

            if (_fields != null)
            {
                _targetStatements.AddRange(_fields);
            }

            if (_variables != null)
            {
                _targetStatements.AddRange(_variables);
            }

            if (_begin != null)
            {
                _targetStatements.AddRange(_begin);
            }

            if (_statements != null)
            {
                _targetStatements.AddRange(_statements);
            }

            if (_end != null)
            {
                _targetStatements.AddRange(_end);
            }
        }
    }
}
