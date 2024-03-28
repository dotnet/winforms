// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This structure is used by IntegrateStatements to put statements in the right place.
/// </summary>
internal class CodeMethodMap
{
    private CodeStatementCollection? _container;
    private CodeStatementCollection? _begin;
    private CodeStatementCollection? _end;
    private CodeStatementCollection? _statements;
    private CodeStatementCollection? _locals;
    private CodeStatementCollection? _fields;
    private CodeStatementCollection? _variables;
    private readonly CodeStatementCollection _targetStatements;

    internal CodeMethodMap(CodeMemberMethod method)
    {
        Method = method;
        _targetStatements = Method.Statements;
    }

    internal CodeMethodMap(CodeStatementCollection targetStatements)
    {
        _targetStatements = targetStatements;
    }

    internal CodeStatementCollection BeginStatements => _begin ??= [];

    internal CodeStatementCollection EndStatements => _end ??= [];

    internal CodeStatementCollection ContainerStatements => _container ??= [];

    internal CodeMemberMethod? Method { get; }

    internal CodeStatementCollection Statements => _statements ??= [];

    internal CodeStatementCollection LocalVariables => _locals ??= [];

    internal CodeStatementCollection FieldAssignments => _fields ??= [];

    // TODO: Should we update RootCodeDomSerializer as well?
    internal CodeStatementCollection VariableAssignments => _variables ??= [];

    internal void Add(CodeStatementCollection statements)
    {
        foreach (CodeStatement statement in statements)
        {
            if (statement.UserData["IContainer"] is "IContainer")
            {
                ContainerStatements.Add(statement);
            }
            else if (statement is CodeAssignStatement { Left: CodeFieldReferenceExpression } fieldAssignment)
            {
                FieldAssignments.Add(fieldAssignment);
            }
            else if (statement is CodeAssignStatement { Left: CodeVariableReferenceExpression } variableAssignment)
            {
                VariableAssignments.Add(variableAssignment);
            }
            else if (statement is CodeVariableDeclarationStatement)
            {
                LocalVariables.Add(statement);
            }
            else
            {
                switch (statement.UserData["statement-ordering"])
                {
                    case "begin":
                        BeginStatements.Add(statement);
                        break;

                    case "end":
                        EndStatements.Add(statement);
                        break;

                    default:
                        Statements.Add(statement);
                        break;
                }
            }
        }
    }

    internal void Combine()
    {
        if (_container is not null)
        {
            _targetStatements.AddRange(_container);
        }

        if (_locals is not null)
        {
            _targetStatements.AddRange(_locals);
        }

        if (_fields is not null)
        {
            _targetStatements.AddRange(_fields);
        }

        if (_variables is not null)
        {
            _targetStatements.AddRange(_variables);
        }

        if (_begin is not null)
        {
            _targetStatements.AddRange(_begin);
        }

        if (_statements is not null)
        {
            _targetStatements.AddRange(_statements);
        }

        if (_end is not null)
        {
            _targetStatements.AddRange(_end);
        }
    }
}
