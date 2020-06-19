// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace WinForms.Common.Tests
{
    public static class CodeDomHelpers
    {
        public static void AssertEqualCodeStatementCollection(CodeStatementCollection expected, CodeStatementCollection actual)
        {
            try
            {
                Assert.Equal(expected.Count, actual.Count);
                for (int i = 0; i < expected.Count; i++)
                {
                    Assert.Equal(GetConstructionString(expected[i]), GetConstructionString(actual[i]));
                }
            }
            catch (Xunit.Sdk.AssertActualExpectedException)
            {
                Console.WriteLine($"Expected: {expected.Count} elements");
                for (int i = 0; i < expected.Count; i++)
                {
                    Console.WriteLine($"- [{i}] {GetConstructionString(expected[i])}");
                }

                Console.WriteLine("");

                Console.WriteLine($"Actual: {actual.Count} elements");
                for (int i = 0; i < actual.Count; i++)
                {
                    Console.WriteLine($"- [{i}] {GetConstructionString(actual[i])}");
                }

                throw;
            }
        }

        public static string GetConstructionString(CodeObject o)
        {
            if (o is CodeStatement cs)
            {
                if (cs.StartDirectives.Count != 0 || cs.EndDirectives.Count != 0)
                {
                    throw new NotImplementedException("Directives not supported.");
                }
            }

            switch (o)
            {
                case CodeVariableDeclarationStatement v:
                    if (v.InitExpression is null)
                    {
                        return $"new CodeVariableDeclarationStatement({GetType(v.Type)}, {GetString(v.Name)});";
                    }

                    return $"new CodeVariableDeclarationStatement({GetType(v.Type)}, {GetString(v.Name)}, {GetConstructionString(v.InitExpression)});";
                case CodeAssignStatement cas:
                    return $"new CodeAssignStatement({GetConstructionString(cas.Left)}, {GetConstructionString(cas.Right)})";
                case CodeVariableReferenceExpression cvre:
                    return $"new CodeVariableReferenceExpression({GetString(cvre.VariableName)})";
                case CodeObjectCreateExpression coce:
                    {
                        if (coce.Parameters.Count == 0)
                        {
                            return $"new CodeObjectCreateExpression({GetType(coce.CreateType)})";
                        }

                        string parameters = string.Join(", ", coce.Parameters.Cast<CodeObject>().Select(o => GetConstructionString(o)));
                        return $"new CodeObjectCreateExpression({GetType(coce.CreateType)}, {parameters})";
                    }

                case CodeCommentStatement ccs:
                    if (ccs.Comment.DocComment)
                    {
                        return $"new CodeCommentStatement({GetString(ccs.Comment.Text)}, true)";
                    }

                    return $"new CodeCommentStatement({GetString(ccs.Comment.Text)})";
                case CodePropertyReferenceExpression cpre:
                    return $"new CodePropertyReferenceExpression({GetConstructionString(cpre.TargetObject)}, {GetString(cpre.PropertyName)})";
                case CodePrimitiveExpression cpe:
                    {
                        if (cpe.Value is null)
                        {
                            return "new CodePrimitiveExpression(null)";
                        }
                        else if (cpe.Value is string s)
                        {
                            return $"new CodePrimitiveExpression({GetString(s)})";
                        }

                        return $"new CodePrimitiveExpression({cpe.Value})";
                    }

                default:
                    throw new NotImplementedException(o.ToString());
            }

            string GetString(string s)
            {
                if (s is null)
                {
                    return "null";
                }
                else if (s.Length == 0)
                {
                    return "string.Empty";
                }

                return $"\"{s}\"";
            }

            string GetType(CodeTypeReference reference)
            {
                Type result = Type.GetType(reference.BaseType);
                if (result != null)
                {
                    return $"typeof({result.Name})";
                }

                return GetString(reference.BaseType);
            }
        }
    }
}