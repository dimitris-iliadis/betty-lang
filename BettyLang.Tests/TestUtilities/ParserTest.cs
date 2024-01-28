using BettyLang.Core.AST;

namespace BettyLang.Tests.TestUtilities
{
    public class ParserTest
    {
        private ASTNode ParseProgram(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            return parser.Parse();
        }

        protected ASTNode ExtractFirstStatement(string code)
        {
            var node = ParseProgram($"main {{ {code} }}");
            var programNode = Assert.IsType<ProgramNode>(node);
            var mainBlock = Assert.IsType<CompoundStatementNode>(programNode.MainBlock);
            Assert.NotEmpty(mainBlock.Statements);
            return mainBlock.Statements[0];
        }

        protected Parser SetupParser(string code)
        {
            var lexer = new Lexer(code);
            return new Parser(lexer);
        }
    }
}