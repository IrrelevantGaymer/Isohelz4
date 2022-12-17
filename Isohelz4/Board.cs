using System;
using System.Collections.Generic;
using System.Text;

namespace Isohelz4 {
    using static GameState;

    public struct Board {
        public static readonly int[,] WinningConfigurations = new int[,] {
            { 0, 1, 2 },
            { 0, 3, 6 },
            { 0, 4, 8 },
            { 1, 4, 7 },
            { 2, 4, 6 },
            { 2, 5, 8 },
            { 3, 4, 5 },
            { 6, 7, 8 }
        };
        
        //pieces array is indexed by [tile, cell] not [row, column]
        GameState[,] pieceState;
        GameState[] tileState;
        GameState boardState;

        private byte currentTile;
        private byte ply;

        public ulong HashKey { get; private set; }
        public bool WhiteTurn { get; private set; }
        public int CurrentTile { get => currentTile; private set => currentTile = (byte) value; }
        public int Ply { get => ply; private set => ply = (byte) value; }

        public void IsValidMove(Move move) {
            if (boardState != None)
                throw new InvalidMoveException(move.ToString() + " cannot be played on a Won/Drawn Board");
            if (tileState[move.Tile] != None)
                throw new InvalidMoveException(
                    move.ToString() + 
                    " cannot be played in the " 
                    + Move.IndexToString(move.Tile) + 
                    " since it is Won/Drawn"
                );
            if (!move.IsFreeMove && move.Tile != currentTile)
                throw new InvalidMoveException(
                    move.ToString() + 
                    " cannot be played since the tile does not match the board's current tile: " 
                    + CurrentTile
                );
            if (pieceState[move.Tile, move.Cell] != None)
                throw new InvalidMoveException(
                    move.ToString() + 
                    " cannot be played since that cell is occupied by a " 
                    + pieceState[move.Tile, move.Cell].ToString() + " piece"
                );

            return;
        }

        public void MakeMove(Move move) {
            try {
                IsValidMove(move);
            } catch (InvalidMoveException e) {
                throw e;
            }

            pieceState[move.Tile, move.Cell] = WhiteTurn ? White : Black;
            UpdateTileState(move.Tile);
            UpdateBoardState();

            WhiteTurn = !WhiteTurn;
            CurrentTile = (byte) (tileState[move.Cell] != None ? 9 : move.Cell);
            Ply++;
        }

        public void UnmakeMove(Move move) {
            boardState = None;
            tileState[move.Tile] = None;
            pieceState[move.Tile, move.Cell] = None;

            WhiteTurn = !WhiteTurn;
            CurrentTile = (byte) (move.IsFreeMove ? 9 : move.Tile);
            Ply--;
        }

        public void MakeNullMove() {
            WhiteTurn = !WhiteTurn;
            Ply++;
        }

        public void UnmakeNullMove() {
            WhiteTurn = !WhiteTurn;
            Ply--;
        }

        private void UpdateTileState(int tile) {
            for (int i = 0; i < WinningConfigurations.GetLength(0); i++) {
                GameState[] temp = new GameState[3];
                for (int j = 0; j < WinningConfigurations.GetLength(1); j++) {
                    temp[j] = pieceState[tile, WinningConfigurations[i, j]];

                    if (j == 0 && temp[j] == None)
                        break;
                    else if (j != 0 && temp[j] != temp[j-1])
                        break;
                    else if (j == 2) {
                        tileState[tile] = temp[j];
                        return;
                    }
                        
                }
            }
        }

        private void UpdateBoardState() {
            for (int i = 0; i < WinningConfigurations.GetLength(0); i++) {
                GameState[] temp = new GameState[3];
                for (int j = 0; j < WinningConfigurations.GetLength(1); j++) {
                    temp[j] = tileState[WinningConfigurations[i, j]];

                    if (j == 0 && temp[j] == None)
                        break;
                    else if (j != 0 && temp[j] != temp[j - 1])
                        break;
                    else if (j == 2) {
                        boardState = temp[j];
                        return;
                    }

                }
            }
        }

        public override string ToString() {
            string output = "";
            
            throw new NotImplementedException();
        }
    }

    public enum GameState {
        None = 0,
        White = 1,
        Black = 2,
        Drawn = White | Black
    }
}
