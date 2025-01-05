using System;

namespace Portfolio.Shared
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Searches for a sequence of characters within an array and returns the index of the first occurrence.
        /// </summary>
        /// <param name="array">The character array to search within.</param>
        /// <param name="block">The sequence of characters to search for.</param>
        /// <returns>
        /// The index of the first occurrence of the sequence within the array, or -1 if the sequence is not found.
        /// </returns>
        public static int GetIndexOf(this char[] array, ReadOnlySpan<char> block)
        {
            int arrayLength = array.Length;
            int valueLength = block.Length;

            if (valueLength == 0 || valueLength > arrayLength)
            {
                return -1;
            }

            for (int i = 0; i <= arrayLength - valueLength; i++)
            {
                int j = 0;
                while (j < valueLength && array[i + j] == block[j])
                {
                    j++;
                }

                if (j == valueLength)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Moves a character in the array to the right by a specified number of positions.
        /// </summary>
        /// <param name="array">The character array.</param>
        /// <param name="startIndex">The index of the character to move.</param>
        /// <param name="distance">The number of positions to move the character to the right.</param>
        public static void MoveCharRight(this char[] array, int startIndex, int distance)
        {
            int targetIndex = startIndex + distance;

            if (targetIndex < array.Length)
            {
                char characterToMove = array[targetIndex];

                for (int i = targetIndex; i > startIndex; i--)
                {
                    array[i] = array[i - 1];
                }

                array[startIndex] = characterToMove;
            }
        }

        /// <summary>
        /// Moves a character in the array to the left by a specified number of positions.
        /// </summary>
        /// <param name="array">The character array.</param>
        /// <param name="startIndex">The index of the character to move.</param>
        /// <param name="distance">The number of positions to move the character to the left.</param>
        public static void MoveCharLeft(this char[] array, int startIndex, int distance)
        {
            int targetIndex = startIndex - distance;

            if (targetIndex >= 0)
            {
                char characterToMove = array[startIndex];
                
                for (int i = startIndex; i > targetIndex; i--)
                {
                    array[i] = array[i - 1];
                }

                array[targetIndex] = characterToMove;
            }
        }
    }
}