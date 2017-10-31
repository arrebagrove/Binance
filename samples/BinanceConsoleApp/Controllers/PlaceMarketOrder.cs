﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account.Orders;

namespace BinanceConsoleApp.Controllers
{
    internal class PlaceMarketOrder : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("market ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 4)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A side, symbol, and quantity are required.");
                return true;
            }

            if (!Enum.TryParse(typeof(OrderSide), args[1], true, out var side))
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A valid order side is required ('buy' or 'sell').");
                return true;
            }

            var symbol = args[2];

            if (!decimal.TryParse(args[3], out var quantity) || quantity <= 0)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A quantity greater than 0 is required.");
                return true;
            }

            decimal stopPrice = 0;
            if (args.Length > 4)
            {
                if (!decimal.TryParse(args[4], out stopPrice) || stopPrice <= 0)
                {
                    lock (Program.ConsoleSync)
                        Console.WriteLine("A stop price greater than 0 is required.");
                    return true;
                }
            }

            var clientOrder = new MarketOrder
            {
                Symbol = symbol,
                Side = (OrderSide)side,
                Quantity = quantity,
                StopPrice = stopPrice
            };

            if (Program.IsOrdersTestOnly)
            {
                await Program.Api.TestPlaceAsync(Program.User, clientOrder, token: token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine($"~ TEST ~ >> MARKET {clientOrder.Side} order (ID: {clientOrder.Id}) placed for {clientOrder.Quantity:0.00000000} {clientOrder.Symbol}");
                }
            }
            else
            {
                var order = await Program.Api.PlaceAsync(Program.User, clientOrder, token: token);

                if (order != null)
                {
                    lock (Program.ConsoleSync)
                    {
                        Console.WriteLine($">> MARKET {order.Side} order (ID: {order.Id}) placed for {order.OriginalQuantity:0.00000000} {order.Symbol} @ {order.Price:0.00000000}");
                    }
                }
            }

            return true;
        }
    }
}
