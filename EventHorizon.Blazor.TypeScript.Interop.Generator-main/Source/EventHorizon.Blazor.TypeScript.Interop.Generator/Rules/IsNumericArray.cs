namespace EventHorizon.Blazor.TypeScript.Interop.Generator.Rules
{
    using System.Linq;
    using EventHorizon.Blazor.TypeScript.Interop.Generator.AstParser.Api;
    using EventHorizon.Blazor.TypeScript.Interop.Generator.Model;

    public class IsNumericArray
        : IRule
    {
        public bool Check(
            Node node
        )
        {
            return JavaScriptTypes.NumberArrayTypes.Any(
                a => a == node.IdentifierStr
            );
        }
    }
}
