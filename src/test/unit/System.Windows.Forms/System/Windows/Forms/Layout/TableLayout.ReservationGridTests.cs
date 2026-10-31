// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;

namespace System.Windows.Forms.Layout.Tests;

public class TableLayoutReservationGridTests
{
    [WinFormsFact]
    public void ReservationGrid_AdvanceRow_EmptyGrid_RemainsEmpty()
    {
        ReservationGridAccessor grid = new();

        grid.AdvanceRow();

        Assert.False(grid.IsReserved(0, 0));
        Assert.Equal(0, grid.PhysicalRowCount);
        Assert.Equal(0, grid.StartIndex);
    }

    [WinFormsFact]
    public void ReservationGrid_AdvanceRow_OneRow_ClearsGrid()
    {
        ReservationGridAccessor grid = new();
        grid.Reserve(2, 0);

        grid.AdvanceRow();

        Assert.False(grid.IsReserved(2, 0));
        Assert.Equal(0, grid.PhysicalRowCount);
        Assert.Equal(0, grid.StartIndex);
    }

    [WinFormsFact]
    public void ReservationGrid_AdvanceRow_MultipleRows_UpdatesLogicalOffsets()
    {
        ReservationGridAccessor grid = new();
        grid.Reserve(0, 0);
        grid.Reserve(1, 1);
        grid.Reserve(2, 2);

        grid.AdvanceRow();

        Assert.True(grid.IsReserved(1, 0));
        Assert.True(grid.IsReserved(2, 1));
        Assert.False(grid.IsReserved(0, 0));
        Assert.False(grid.IsReserved(2, 2));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(8)]
    public void ReservationGrid_IsReserved_OffsetBeyondLiveRows_ReturnsFalse(int advances)
    {
        ReservationGridAccessor grid = new();
        grid.Reserve(0, 7);
        for (int row = 0; row < advances; row++)
        {
            grid.AdvanceRow();
        }

        Assert.False(grid.IsReserved(0, 8 - advances));
        Assert.False(grid.IsReserved(0, int.MaxValue - 1));
        Assert.False(grid.IsReserved(0, int.MaxValue));
    }

    [WinFormsFact]
    public void ReservationGrid_Reserve_AfterPartialAdvance_UsesLogicalOffset()
    {
        ReservationGridAccessor grid = new();
        grid.Reserve(0, 0);
        grid.Reserve(1, 1);
        grid.Reserve(2, 4);
        grid.AdvanceRow();

        grid.Reserve(3, 5);

        Assert.True(grid.IsReserved(1, 0));
        Assert.True(grid.IsReserved(2, 3));
        Assert.True(grid.IsReserved(3, 5));
        Assert.False(grid.IsReserved(3, 4));
        Assert.Equal(1, grid.StartIndex);
    }

    [WinFormsFact]
    public void ReservationGrid_Reserve_AfterConsumingAllRows_StartsAtZero()
    {
        ReservationGridAccessor grid = new();
        grid.Reserve(0, 0);
        grid.Reserve(1, 1);
        grid.AdvanceRow();
        grid.AdvanceRow();

        grid.Reserve(4, 0);

        Assert.True(grid.IsReserved(4, 0));
        Assert.False(grid.IsReserved(1, 0));
        Assert.Equal(1, grid.PhysicalRowCount);
        Assert.Equal(0, grid.StartIndex);
    }

    [WinFormsFact]
    public void ReservationGrid_Reserve_LargeOffsetAndMultipleColumns_PreservesReservations()
    {
        ReservationGridAccessor grid = new();

        grid.Reserve(0, 4_096);
        grid.Reserve(63, 4_096);

        Assert.True(grid.IsReserved(0, 4_096));
        Assert.True(grid.IsReserved(63, 4_096));
        Assert.False(grid.IsReserved(62, 4_096));
        Assert.False(grid.IsReserved(63, 4_095));
    }

    [WinFormsFact]
    public void ReservationGrid_AdvanceRow_WhenDeadPrefixReachesLiveRows_Compacts()
    {
        ReservationGridAccessor grid = new();
        for (int row = 0; row < 8; row++)
        {
            grid.Reserve(row, row);
        }

        for (int row = 0; row < 3; row++)
        {
            grid.AdvanceRow();
        }

        Assert.Equal(3, grid.StartIndex);
        Assert.Equal(8, grid.PhysicalRowCount);

        grid.AdvanceRow();

        Assert.Equal(0, grid.StartIndex);
        Assert.Equal(4, grid.PhysicalRowCount);
        for (int row = 0; row < 4; row++)
        {
            Assert.True(grid.IsReserved(row + 4, row));
        }
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(1_337)]
    public void ReservationGrid_RandomOperations_MatchReferenceModel(int seed)
    {
        const int Columns = 12;
        const int RowOffsets = 72;
        ReservationGridAccessor actual = new();
        ReferenceReservationGrid expected = new();
        Random random = new(seed);

        for (int operation = 0; operation < 1_000; operation++)
        {
            int column = random.Next(Columns);
            int rowOffset = random.Next(RowOffsets);

            switch (random.Next(4))
            {
                case 0:
                case 1:
                    if (!expected.IsReserved(column, rowOffset))
                    {
                        expected.Reserve(column, rowOffset);
                        actual.Reserve(column, rowOffset);
                    }

                    break;
                case 2:
                    expected.AdvanceRow();
                    actual.AdvanceRow();
                    break;
                default:
                    Assert.Equal(expected.IsReserved(column, rowOffset), actual.IsReserved(column, rowOffset));
                    break;
            }

            for (int probeColumn = 0; probeColumn < Columns; probeColumn++)
            {
                for (int probeRow = 0; probeRow < RowOffsets; probeRow++)
                {
                    Assert.Equal(
                        expected.IsReserved(probeColumn, probeRow),
                        actual.IsReserved(probeColumn, probeRow));
                }
            }
        }
    }

    private sealed class ReservationGridAccessor
    {
        private static readonly Type s_reservationGridType = typeof(TableLayout).GetNestedType(
            "ReservationGrid",
            BindingFlags.NonPublic)!;
        private static readonly MethodInfo s_advanceRowMethod = s_reservationGridType.GetMethod("AdvanceRow")!;
        private static readonly MethodInfo s_isReservedMethod = s_reservationGridType.GetMethod("IsReserved")!;
        private static readonly MethodInfo s_reserveMethod = s_reservationGridType.GetMethod("Reserve")!;
        private static readonly FieldInfo s_rowsField = s_reservationGridType.GetField(
            "_rows",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static readonly FieldInfo s_startIndexField = s_reservationGridType.GetField(
            "_startIndex",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly object _instance = Activator.CreateInstance(s_reservationGridType, nonPublic: true)!;
        private readonly Action _advanceRow;
        private readonly Func<int, int, bool> _isReserved;
        private readonly Action<int, int> _reserve;

        public ReservationGridAccessor()
        {
            _advanceRow = s_advanceRowMethod.CreateDelegate<Action>(_instance);
            _isReserved = s_isReservedMethod.CreateDelegate<Func<int, int, bool>>(_instance);
            _reserve = s_reserveMethod.CreateDelegate<Action<int, int>>(_instance);
        }

        public int PhysicalRowCount => ((ICollection)s_rowsField.GetValue(_instance)!).Count;

        public int StartIndex => (int)s_startIndexField.GetValue(_instance)!;

        public void AdvanceRow() => _advanceRow();

        public bool IsReserved(int column, int rowOffset) => _isReserved(column, rowOffset);

        public void Reserve(int column, int rowOffset) => _reserve(column, rowOffset);
    }

    private sealed class ReferenceReservationGrid
    {
        private int _numColumns = 1;
        private readonly List<BitArray> _rows = [];

        public bool IsReserved(int column, int rowOffset) =>
            rowOffset < _rows.Count && column < _rows[rowOffset].Length && _rows[rowOffset][column];

        public void Reserve(int column, int rowOffset)
        {
            while (rowOffset >= _rows.Count)
            {
                _rows.Add(new BitArray(_numColumns));
            }

            if (column >= _rows[rowOffset].Length)
            {
                _rows[rowOffset].Length = column + 1;
                _numColumns = Math.Max(_numColumns, column + 1);
            }

            _rows[rowOffset][column] = true;
        }

        public void AdvanceRow()
        {
            if (_rows.Count > 0)
            {
                _rows.RemoveAt(0);
            }
        }
    }
}
