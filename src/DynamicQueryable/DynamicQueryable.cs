using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    private static Expression CreateLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));

        var types = new[] { source.ElementType };
        var normalizedExpression = NormalizeProjectionExpression(expression!);
        var lambda = Evaluator.ToLambda(normalizedExpression, types, variables, settings, values);

        return Expression.Call(
            typeof(Queryable),
            method,
            generic ? [source.ElementType, lambda.Body.Type] : types,
            source.Expression,
            Expression.Quote(lambda)
        );
    }

    private static Expression CreateExpression(IQueryable source, string method, params Expression[] expressions) {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return Expression.Call(
            typeof(Queryable),
            method,
            [source.ElementType],
            new[] { source.Expression }.Concat(expressions).ToArray()
        );
    }

    private static IQueryable Handle(IQueryable source, string method) {
        var expression = CreateExpression(source, method);
        return source.Provider.CreateQuery(expression);
    }

    private static IQueryable HandleConstant(IQueryable source, string method, object value) {
        var expression = CreateExpression(source, method, Expression.Constant(value));
        return source.Provider.CreateQuery(expression);
    }

    private static IQueryable HandleLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        var lambda = CreateLambda(source, method, expression, generic, variables, values, settings);
        return source.Provider.CreateQuery(lambda);
    }

    private static object? Execute(IQueryable source, string method) {
        var expression = CreateExpression(source, method);
        return source.Provider.Execute(expression);
    }

    private static object? ExecuteLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        var lambda = CreateLambda(source, method, expression, generic, variables, values, settings);
        return source.Provider.Execute(lambda);
    }

    private static object? ExecuteOptionalExpression(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings)
        => string.IsNullOrEmpty(expression)
            ? Execute(source, method)
            : ExecuteLambda(source, method, expression, generic, variables, values, settings);

    private static object? ExecuteConstant(IQueryable source, string method, object value) {
        var expression = CreateExpression(source, method, Expression.Constant(value));
        return source.Provider.Execute(expression);
    }

    private static string NormalizeProjectionExpression(string expression) {
        if (string.IsNullOrWhiteSpace(expression)) {
            return expression;
        }

        var arrowIndex = expression.IndexOf("=>", StringComparison.Ordinal);
        if (arrowIndex < 0) {
            var normalizedBody = TryNormalizeNewProjection(expression.Trim());
            return normalizedBody ?? expression;
        }

        var header = expression.Substring(0, arrowIndex + 2);
        var body = expression.Substring(arrowIndex + 2);
        var normalized = TryNormalizeNewProjection(body.TrimStart());
        if (normalized == null) {
            return expression;
        }

        var leadingWhitespaceLength = body.Length - body.TrimStart().Length;
        return header + body.Substring(0, leadingWhitespaceLength) + normalized;
    }

    private static string? TryNormalizeNewProjection(string body) {
        if (!StartsWithNewProjection(body, out var openParenthesisIndex)) {
            return null;
        }

        var closeParenthesisIndex = FindMatchingParenthesis(body, openParenthesisIndex);
        if (closeParenthesisIndex < 0 || !string.IsNullOrWhiteSpace(body.Substring(closeParenthesisIndex + 1))) {
            return null;
        }

        var inner = body.Substring(openParenthesisIndex + 1, closeParenthesisIndex - openParenthesisIndex - 1);
        var members = SplitTopLevel(inner);
        if (members.Count == 0) {
            return null;
        }

        var rewrittenMembers = new List<string>(members.Count);
        for (var i = 0; i < members.Count; i++) {
            var member = members[i].Trim();
            if (string.IsNullOrWhiteSpace(member)) {
                continue;
            }

            var (valueExpression, alias) = SplitAlias(member);
            var normalizedValueExpression = NormalizeAggregateItSyntax(valueExpression.Trim());
            var finalAlias = string.IsNullOrWhiteSpace(alias) ? DeriveMemberName(valueExpression, i + 1) : alias;
            rewrittenMembers.Add($"{finalAlias} = {normalizedValueExpression}");
        }

        if (rewrittenMembers.Count == 0) {
            return null;
        }

        return $"new {{ {string.Join(", ", rewrittenMembers)} }}";
    }

    private static (string Expression, string? Alias) SplitAlias(string member) {
        var index = FindTopLevelAsKeyword(member);
        if (index < 0) {
            return (member, null);
        }

        var expression = member.Substring(0, index).Trim();
        var alias = member.Substring(index + 2).Trim();
        return string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(alias)
            ? (member, null)
            : (expression, alias);
    }

    private static int FindTopLevelAsKeyword(string text) {
        var parenDepth = 0;
        var bracketDepth = 0;
        var braceDepth = 0;

        for (var i = 0; i <= text.Length - 4; i++) {
            var current = text[i];
            switch (current) {
                case '(':
                    parenDepth++;
                    continue;
                case ')':
                    parenDepth--;
                    continue;
                case '[':
                    bracketDepth++;
                    continue;
                case ']':
                    bracketDepth--;
                    continue;
                case '{':
                    braceDepth++;
                    continue;
                case '}':
                    braceDepth--;
                    continue;
            }

            if (parenDepth != 0 || bracketDepth != 0 || braceDepth != 0) {
                continue;
            }

            if ((current == 'a' || current == 'A') && (text[i + 1] == 's' || text[i + 1] == 'S') &&
                i > 0 && i + 2 < text.Length && char.IsWhiteSpace(text[i - 1]) && char.IsWhiteSpace(text[i + 2])) {
                if (!char.IsWhiteSpace(text[i - 1])) {
                    continue;
                }

                return i;
            }
        }

        return -1;
    }

    private static string DeriveMemberName(string expression, int index) {
        var trimmed = expression.Trim();

        var invocationOpenIndex = FindInvocationOpenIndex(trimmed);
        if (invocationOpenIndex > 0) {
            var token = trimmed.Substring(0, invocationOpenIndex).Trim();
            var tokenParts = token.Split('.');
            var functionName = tokenParts[tokenParts.Length - 1].Trim();
            if (IsIdentifier(functionName)) {
                return functionName;
            }
        }

        var parts = trimmed.Split('.');
        var lastPart = parts[parts.Length - 1].Trim();
        if (IsIdentifier(lastPart)) {
            return lastPart;
        }

        return $"Item{index}";
    }

    private static string NormalizeAggregateItSyntax(string expression) {
        var normalized = Regex.Replace(
            expression,
            @"\b(Sum|Avg|Average|Min|Max)\s*\(\s*it\.(?<member>[^)]*)\)",
            m => $"{m.Groups[1].Value}(x => x.{m.Groups["member"].Value.Trim()})",
            RegexOptions.IgnoreCase
        );

        return Regex.Replace(
            normalized,
            @"\bAvg\s*\(",
            "Average(",
            RegexOptions.IgnoreCase
        );
    }

    private static int FindInvocationOpenIndex(string expression) {
        var parenDepth = 0;
        for (var i = 0; i < expression.Length; i++) {
            if (expression[i] == '(') {
                if (parenDepth == 0) {
                    return i;
                }

                parenDepth++;
            }
        }

        return -1;
    }

    private static bool IsIdentifier(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return false;
        }

        if (!(char.IsLetter(value[0]) || value[0] == '_')) {
            return false;
        }

        for (var i = 1; i < value.Length; i++) {
            if (!(char.IsLetterOrDigit(value[i]) || value[i] == '_')) {
                return false;
            }
        }

        return true;
    }

    private static bool StartsWithNewProjection(string text, out int openParenthesisIndex) {
        openParenthesisIndex = -1;
        var i = 0;

        while (i < text.Length && char.IsWhiteSpace(text[i])) {
            i++;
        }

        if (i + 3 >= text.Length) {
            return false;
        }

        if (!(text[i] == 'n' || text[i] == 'N') || !(text[i + 1] == 'e' || text[i + 1] == 'E') || !(text[i + 2] == 'w' || text[i + 2] == 'W')) {
            return false;
        }

        i += 3;
        while (i < text.Length && char.IsWhiteSpace(text[i])) {
            i++;
        }

        if (i >= text.Length || text[i] != '(') {
            return false;
        }

        openParenthesisIndex = i;
        return true;
    }

    private static int FindMatchingParenthesis(string text, int openParenthesisIndex) {
        var depth = 0;
        for (var i = openParenthesisIndex; i < text.Length; i++) {
            if (text[i] == '(') {
                depth++;
                continue;
            }

            if (text[i] == ')') {
                depth--;
                if (depth == 0) {
                    return i;
                }
            }
        }

        return -1;
    }

    private static List<string> SplitTopLevel(string input) {
        var result = new List<string>();
        var builder = new StringBuilder();
        var parenDepth = 0;
        var bracketDepth = 0;
        var braceDepth = 0;

        for (var i = 0; i < input.Length; i++) {
            var current = input[i];
            switch (current) {
                case '(': parenDepth++; break;
                case ')': parenDepth--; break;
                case '[': bracketDepth++; break;
                case ']': bracketDepth--; break;
                case '{': braceDepth++; break;
                case '}': braceDepth--; break;
            }

            if (current == ',' && parenDepth == 0 && bracketDepth == 0 && braceDepth == 0) {
                result.Add(builder.ToString());
                builder.Clear();
                continue;
            }

            builder.Append(current);
        }

        if (builder.Length > 0) {
            result.Add(builder.ToString());
        }

        return result;
    }
}