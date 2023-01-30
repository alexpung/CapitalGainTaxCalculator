﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapitalGainCalculator.Enum;
using NodaMoney;

namespace CapitalGainCalculator.Model
{
    public abstract record TaxEvent
    {
        public string AssetName { get; set; }
        public DateTime Date { get; set; }
    }

    public record Trade : TaxEvent
    {
        public TradeType BuySell { get; set; }
        public decimal Quantity { get; set; }
        public DescribedMoney Proceed { get; set; }
        public IEnumerable<DescribedMoney>? Expenses { get; set; }
    }

    public record Dividend : TaxEvent
    {
        public DividendType DividendType { get; set; }
        public RegionInfo CompanyLocation { get; set; }
        public DescribedMoney Proceed { get; set; }
    }

    public abstract record CorporateAction : TaxEvent
    {
    }

    public record StockSplit : CorporateAction
    {
        public ushort NumberBeforeSplit { get; set; }
        public ushort NumberAfterSplit { get; set; }
    }

    public record DescribedMoney
    {
        public string? Description { get; set; }
        public Money Amount { get; set; }

        public Decimal FxRate { get; set; }
    }

}
