using System;
using System.Collections.Generic;
using System.Text;

namespace Isohelz4 {
    public struct Move {
        public static readonly ushort InvalidMove = 0b1111111111;

        public static readonly ushort CaptureMoveBitMask = 0b1000000000;
        public static readonly ushort FreeMoveBitMask = 0b0100000000;
        public static readonly ushort TileBitMask = 0b0011110000;
        public static readonly ushort CellBitMask = 0b0000001111;

        public ushort MoveBits { get; private set; }

        public bool IsCaptureMove { get => (MoveBits & CaptureMoveBitMask) != 0; }
        public bool IsFreeMove { get => (MoveBits & FreeMoveBitMask) != 0; }
        public int Tile { get => (MoveBits & TileBitMask) >> 4; }
        public int Cell { get => MoveBits & CellBitMask; }

        public Move (int tile, int cell, bool free) {
            MoveBits = (ushort) ((free ? CaptureMoveBitMask : 0) | tile << 4 | cell);
        }

        public void SetCaptureMove() {
            MoveBits |= CaptureMoveBitMask;
        }

        public void SetQuietMove() {
            MoveBits &= (ushort) (CaptureMoveBitMask - 1);
        }

        public override string ToString() {
            string output = "";

            output += IndexToString(Tile) + ":";
            output += IndexToString(Cell);

            return output;
        }

        public static string IndexToString(int index) {
            return index switch {
                0 => "tl",
                1 => "te",
                2 => "tr",
                3 => "le",
                4 => "c",
                5 => "re",
                6 => "bl",
                7 => "be",
                8 => "br",
                _ => ""
            };
        }
    }

    public class InvalidMoveException : Exception {
        public InvalidMoveException(string message) : base(message) { }
    }
}
