﻿using System.Diagnostics;
using GameSolver.Core;
using GameSolver.Solver;
using GameSolver.Solver.ShortestPath;

namespace GameSolver.Benchmark;

public sealed class SolverBenchmark
{
    private readonly int _n;

    private readonly IEnumerable<IShortestPathSolver> _solvers;

    private const string Map1 = @"
        ....*..xx
 		.x.x.x.xx
 		..*...*..
 		.x.x.x.x.
 		P.......G
    ";
    
    public SolverBenchmark(int n)
    {
        _n = n;
        var gameBuilder = new GameBuilder(Map1, Direction.Up);
        var game = gameBuilder.Instance;
        
        _solvers = new IShortestPathSolver[]
        {
            new BreadthFirstSearch(game),
            new IterativeDeepeningDepthFirstSearch(game)
        };
    }

    private void TimeAndMemoryBenchmark()
    {
        var timeResults = new long[_n];
        var peakMemoryResults = new long[_n];

        foreach (IShortestPathSolver solver in _solvers)
        {
            for (int i = 0; i < _n; i++)
            {
                var watch = Stopwatch.StartNew();

                solver.Solve();
            
                watch.Stop();
                long peakWorkingSet = Process.GetCurrentProcess().PeakWorkingSet64 / 1024;

                timeResults[i] = watch.ElapsedMilliseconds;
                peakMemoryResults[i] = peakWorkingSet;
            }
            
            Console.WriteLine($"{solver.GetType()} benchmark");
            Console.WriteLine($"Average time usage: {timeResults.Sum() / timeResults.Length} ms");
            Console.WriteLine($"Average peak memory usage: {peakMemoryResults.Sum() / peakMemoryResults.Length} KB");
        }
    }
    
    public void Run()
    {
        TimeAndMemoryBenchmark();
    }
}