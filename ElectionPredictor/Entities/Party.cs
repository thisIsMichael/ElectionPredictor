using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public enum Party
    {
        Con,
        Lab,
        LibDem,
        UKIP,
        Brexit,
        PlaidCymru,
        SNP,
        Green,
        Other,
        None
    }

    public static class PartyExtensions
    {
        public static bool IsUnionist(this Party party)
        {
            return party == Party.Con || party == Party.Lab || party == Party.LibDem || party == Party.UKIP || party == Party.Other || party == Party.Brexit;
        }
    }
}
