namespace ReleaseToolBuild.Components
{
    public static class Logging
    {
        /// <summary>
        /// Writes a custom colored message to the console, adds a new line at the end.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreColor">The foreground color of the message.</param>
        /// <param name="backColor">The background color of the message.</param>
        public static void WriteColoredLine(string message, ConsoleColor foreColor, ConsoleColor backColor = ConsoleColor.Black)
        {
            // Get the current console colors
            ConsoleColor oldForeColor = Console.ForegroundColor;
            ConsoleColor oldBackColor = Console.BackgroundColor;

            // Set the new colors of the console to the custom colors
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;

            // Write the message to the console
            Console.WriteLine($"[{DateTime.Now}] " + message);

            // Reset the console colors to the original colors
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
        }

        /// <summary>
        /// Writes a custom colored message to the console.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreColor">The foreground color of the message.</param>
        /// <param name="backColor">The background color of the message.</param>
        public static void WriteColored(string message, ConsoleColor foreColor, ConsoleColor backColor = ConsoleColor.Black)
        {
            // Get the current console colors
            ConsoleColor oldForeColor = Console.ForegroundColor;
            ConsoleColor oldBackColor = Console.BackgroundColor;

            // Set the new colors of the console to the custom colors
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;

            // Write the message to the console
            Console.Write($"[{DateTime.Now}] " + message);

            // Reset the console colors to the original colors
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
        }

        /// <summary>
        /// Writes a message to the console at a specific position and resets the cursor position.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="x">The x (left) position to write the message.</param>
        /// <param name="y">The y (top) position to write the message.</param>
        /// <param name="foreColor">The foreground color of the message.</param>
        /// <param name="backColor">The background color of the message.</param>
        public static void WritePosition(string message, int x, int y, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
        {
            // Get the old cursor position
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;
            Console.SetCursorPosition(x, y);

            // Get the current console colors
            ConsoleColor oldForeColor = Console.ForegroundColor;
            ConsoleColor oldBackColor = Console.BackgroundColor;

            // Set the new colors of the console to the custom colors
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;

            // Write the message to the console
            Console.WriteLine(message);

            // Reset the console colors to the original colors
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;

            // Reset the cursor position
            Console.SetCursorPosition(oldX, oldY);
        }

        /// <summary>
        /// Pauses the console and waits for a key press.
        /// </summary>
        /// <param name="showText">Write a "Press any key to continue" message to the console when paused.</param>
        public static void Pause(bool showText = true)
        {
            if (showText) // If the showText parameter is true write the message
                Console.WriteLine("Press any key to continue...");
            // Wait for a key press
            Console.ReadKey(true);
        }
    }
}
