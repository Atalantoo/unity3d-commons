﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

// API *******************************************************

public interface IGameManager
{
    Game Start(GameStartInput input);
    Game Turn(GameTurnInput input);
    Game Reload(Game input);
}
public class Game
{
    public int Width;
    public int Height;
    public int[,] Board;
    public GameState State;
    public int Score;
    public Movement[] AvailableMoves;
}
public class GameStartInput
{
    public int Width;
    public int Height;
}
public class GameTurnInput
{
    public Movement Move;
}
public enum GameState { Playing, Won, Loss }
public enum Movement { Left, Right, Top, Bottom }

// IMPL *******************************************************

public class GameManager : Game, IGameManager
{
    public Game Start(GameStartInput input)
    { // Starting_the_Game_Turn
        { // Setup_Phase
            Clean_game_states();
            Update_input(input);
            Fill_with_zeros_items();
            Create_random_item();
            Create_random_item();
        }
        { // End
            Update_score();
            //  TODO     .Next(  available_moves )))
        }
        return Return();
    }

    public Game Turn(GameTurnInput input)
    { // Player Turn
        { // Beginning phase
            Get_Input(input);
        }
        {  // Move phase
            Merge_Items();
            Move_Items();
            Create_random_item();
        }
        { // End
            Update_score();
            //  TODO     .Next(  available_moves )))
        }
        return Return();
    }

    public Game Reload(Game input)
    { // Starting_the_Game_Turn
        { // Setup_Phase
            Clean_game_states();
            Reload_Board(input);
        }
        { // End
            Update_score();
            //  TODO     .Next(  available_moves )))
        }
        return Return();
    }

    // TODO next(Turns.Get( "Ending the Game" )
    //  TODO .Start( end )
    //  TODO .Next(  available_moves )))   

    //  OBJECT *******************************************************
    // TODO external Game object ?

    private const int NEW = 2;
    private const int FREE = 0;
    private class Point { public int y; public int x; public Point(int y, int x) { this.y = y; this.x = x; } }
    public enum HorizontalMovement { Left = -1, Right = 1 }
    public enum VerticalMovement { Top = -1, Bottom = 1 }

    private Movement Move;
    // TODO string[][][] histoStates;
    // TODO string[] histoEvents;

    void Update_input(GameStartInput input)
    {
        Width = input.Width;
        Height = input.Height;
    }

    void Clean_game_states()
    {
        Width = -1;
        Height = -1;
        Board = null;
        Score = -1;
    }

    void Update_score()
    {
        State = GameState.Playing;
        Score = 0;
    }

    void Fill_with_zeros_items()
    {
        Board = new int[Height, Width];
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Board[y, x] = FREE;
    }

    void Create_random_item()
    {
        Point[] freeItems = GetEmptyItems();
        int i = new Random().Next(0, freeItems.Length);
        Board[freeItems[i].y, freeItems[i].x] = NEW;
    }

    void Reload_Board(Game i)
    {
        Width = i.Width;
        Height = i.Height;
        Board = i.Board;
    }

    Game Return()
    {
        return new Game()
        {
            Width = Width,
            Height = Height,
            Board = Board
        };
    }

    void Get_Input(GameTurnInput input)
    {
        Move = input.Move;
    }

    void Merge_Items()
    {
        if (Enum.IsDefined(typeof(HorizontalMovement), Move.ToString()))
            Merge_horizontal((HorizontalMovement)Enum.Parse(typeof(HorizontalMovement), Move.ToString()));
        else if (Enum.IsDefined(typeof(VerticalMovement), Move.ToString()))
            Merge_vertical((VerticalMovement)Enum.Parse(typeof(VerticalMovement), Move.ToString()));
    }

    void Move_Items()
    {
        if (Enum.IsDefined(typeof(HorizontalMovement), Move.ToString()))
            Move_horizontal((HorizontalMovement)Enum.Parse(typeof(HorizontalMovement), Move.ToString()));
        else if (Enum.IsDefined(typeof(VerticalMovement), Move.ToString()))
            Move_vertical((VerticalMovement)Enum.Parse(typeof(VerticalMovement), Move.ToString()));
    }

    void Merge_horizontal(HorizontalMovement move)
    {
        for (int y = 0; y < Height; y++)
            foreach (int x in OrderRange_horizontal(move, Width))
                if (!FREE.Equals(Board[y, x]))
                {
                    Point found = FindSameItem_Horizontal(y, x, move, Board[y, x]);
                    if (found != null)
                    {
                        Board[y, x] = Board[y, x] * 2;
                        Board[found.y, found.x] = FREE;
                    }
                }
    }
    void Merge_vertical(VerticalMovement move)
    {
        for (int x = 0; x < Height; x++)
            foreach (int y in OrderRange_vertical(move, Width))
                if (!FREE.Equals(Board[y, x]))
                {
                    Point found = FindSameItem_vertical(y, x, move, Board[y, x]);
                    if (found != null)
                    {
                        Board[y, x] = Board[y, x] * 2;
                        Board[found.y, found.x] = FREE;
                    }
                }
    }

    void Move_horizontal(HorizontalMovement move)
    {
        for (int y = 0; y < Height; y++)
            foreach (int x in OrderRange_horizontal(move, Width))
                if (!FREE.Equals(Board[y, x]))
                {
                    Point found = FindEmptyItem_Horizontal(y, x, move);
                    if (found != null)
                    {
                        Board[found.y, found.x] = Board[y, x];
                        Board[y, x] = FREE;
                    }
                }
    }
    void Move_vertical(VerticalMovement move)
    {
        for (int x = 0; x < Height; x++)
            foreach (int y in OrderRange_vertical(move, Width))
                if (!FREE.Equals(Board[y, x]))
                {
                    Point found = FindEmptyItem_vertical(y, x, move);
                    if (found != null)
                    {
                        Board[found.y, found.x] = Board[y, x];
                        Board[y, x] = FREE;
                    }
                }
    }

    // FUNCTION(S ) *******************************************************


    Point FindSameItem_Horizontal(int y, int x, HorizontalMovement Move, int value)
    {
        int i = x + (int)Move;
        while (i >= 0 && i < Width)
            if (value.Equals(Board[y, i]))
                return new Point(y, i);
            else if (!FREE.Equals(Board[y, i]))
                break;
            else
                i += (int)Move;
        return null;
    }
    Point FindSameItem_vertical(int y, int x, VerticalMovement Move, int value)
    {
        int i = y + (int)Move;
        while (i >= 0 && i < Width)
            if (value.Equals(Board[i, x]))
                return new Point(i, x);
            else if (!FREE.Equals(Board[i, x]))
                break;
            else
                i += (int)Move;
        return null;
    }

    Point FindEmptyItem_Horizontal(int y, int x, HorizontalMovement Move)
    {
        int i = x;
        bool emptyItemFound = true;
        while (emptyItemFound)
        {
            int next = i + (int)Move;
            emptyItemFound = (next >= 0 && next < Width) && FREE.Equals(Board[y, next]);
            if (emptyItemFound)
                i = next;
        }
        return new Point(y, i);
    }
    Point FindEmptyItem_vertical(int y, int x, VerticalMovement Move)
    {
        int i = y;
        bool emptyItemFound = true;
        while (emptyItemFound)
        {
            int next = i + (int)Move;
            emptyItemFound = (next >= 0 && next < Width) && FREE.Equals(Board[next, x]);
            if (emptyItemFound)
                i = next;
        }
        return new Point(i, x);
    }

    IEnumerable<int> OrderRange_horizontal(HorizontalMovement move, int width)
    {
        return ((int)move == -1) ? Enumerable.Range(0, width) : Enumerable.Range(0, width).Reverse();
    }
    IEnumerable<int> OrderRange_vertical(VerticalMovement move, int width)
    {
        return ((int)move == -1) ? Enumerable.Range(0, width) : Enumerable.Range(0, width).Reverse();
    }

    Point[] GetEmptyItems()
    {
        List<Point> res = new List<Point>();
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (FREE.Equals(Board[y, x]))
                    res.Add(new Point(y, x));
        return res.ToArray();
    }

}