namespace SharpMC
{
    public class ChatColor
    {
        public const char COLOR_CHARACTER = '§';
        
        public static readonly ChatColor DARK_RED = new ChatColor('4');
        public static readonly ChatColor RED = new ChatColor('c');
        public static readonly ChatColor GOLD = new ChatColor('6');
        public static readonly ChatColor YELLOW = new ChatColor('e');
        public static readonly ChatColor DARK_GREEN = new ChatColor('2');
        public static readonly ChatColor GREEN = new ChatColor('a');
        public static readonly ChatColor AQUA = new ChatColor('b');
        public static readonly ChatColor DARK_AQUA = new ChatColor('3');
        public static readonly ChatColor DARK_BLUE = new ChatColor('1');
        public static readonly ChatColor BLUE = new ChatColor('9');
        public static readonly ChatColor LIGHT_PURPLE = new ChatColor('d');
        public static readonly ChatColor DARK_PURPLE = new ChatColor('5');
        public static readonly ChatColor WHITE = new ChatColor('f');
        public static readonly ChatColor GRAY = new ChatColor('7');
        public static readonly ChatColor DARK_GRAY = new ChatColor('7');
        public static readonly ChatColor BLACK = new ChatColor('0');
        
        public static readonly ChatColor OBFUSCATED = new ChatColor('k');
        public static readonly ChatColor BOLD = new ChatColor('l');
        public static readonly ChatColor STRIKETHROUGH = new ChatColor('m');
        public static readonly ChatColor UNDERLINE = new ChatColor('n');
        public static readonly ChatColor ITALIC = new ChatColor('o');
        public static readonly ChatColor RESET = new ChatColor('r');
        
        public char Character { get; }

        private ChatColor(char c)
        {
            Character = c;
        }

        public override string ToString()
        {
            return COLOR_CHARACTER + Character.ToString();
        }

        public static string ReplaceAlternativeColorCodes(char alt, string text)
        {
            return text.Replace(alt, COLOR_CHARACTER);
        }
    }
}