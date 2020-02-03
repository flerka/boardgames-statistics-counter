using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public static class TextMessageGrammar
    {
        public static readonly Parser<IEnumerable<IEnumerable<string>>> MessageParser =
            Parse.AnyChar.Except(Parse.Char(',')
                .Or(Parse.Char('\n'))).Many().Text()
                .DelimitedBy(Parse.Char(',').Token())
                .DelimitedBy(Parse.Char('\n'));
    }
}
