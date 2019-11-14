using ElectionPredictor.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectionPredictor
{
    public static class ConstituencyManager
    {
        private static string constuencyDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "constituencies.json");

        public static void GetConstituencies(Election electionToPredict)
        {
            dynamic loadedJson; 
            using (StreamReader r = new StreamReader(constuencyDataPath))
            {
                string json = r.ReadToEnd();
                //List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                loadedJson = JsonConvert.DeserializeObject(json);
            }

            var constituencies = loadedJson.constituencies;

            List<Constituency> constituenciesList = new List<Constituency>();

            foreach (var constituency in constituencies)
            {
                var c = new Constituency();

                var regionString = (string)constituency.region;
                if (regionString == "Northern Ireland")
                    continue; // ignore NI

                c.Region = GetRegionFromString(regionString);

                c.ONSReference = (string)constituency.onsRef;

                var euRef = constituency.referendums.eu;
                c.ReferendumResults = new Dictionary<ReferendumResult, double>
                {
                    { ReferendumResult.Leave, (double)euRef.leave },
                    { ReferendumResult.Remain, (double)euRef.remain }
                };

                var electionResult = electionToPredict == Election.e2017 ? constituency.elections["2015"] : constituency.elections["2017"];
                c.PreviousVote = new Dictionary<Party, double>();
                foreach (var result in electionResult)
                {
                    Party party = GetPartyFromString((string)result.Name);
                    c.PreviousVote.Add(party, (double)result.Value);
                }

                var y = false;
            }

            var x = true;
        }

        private static Party GetPartyFromString(string party)
        {
            switch (party)
            {
                case "Scottish National Party":
                    return Party.SNP;
                case "Conservative":
                    return Party.Con;
                case "Labour":
                case "Lab Co-op":
                    return Party.Lab;
                case "Lib Dem":
                    return Party.LibDem;
                case "UKIP":
                    return Party.UKIP;
                case "Green":
                    return Party.Green;
                case "Plaid Cymru":
                    return Party.PlaidCymru;
                case "Speaker":
                case "Loony":
                case "JMB":
                case "National Health Action Party":
                case "Trade Unionist and Socialist Coalition":
                case "Eng Dem":
                case "Vapers":
                case "Yorks":
                case "Independent":
                case "PSP":
                case "AP":
                case "Rep Soc":
                case "Red Flag Anti-Corruption":
                case "30-50":
                case "Whig":
                case "Cannabis is Safer than Alcohol":
                case "Community":
                case "Christian":
                case "Respect":
                case "Comm Brit":
                case "Lib GB":
                case "Soc Dem":
                case "Northern":
                case "IE":
                case "Pilgrim":
                case "BNP":
                case "ND":
                case "Bournemouth":
                case "Patria":
                case "Brit Dem":
                case "SPGB":
                case "Bristol":
                case "LU":
                case "Song":
                case "WRP":
                case "Meb Ker":
                case "RTP":
                case "National Front":
                case "CPA":
                case "Above":
                case "Lib":
                case "Peace":
                case "Class War":
                case "Ch M":
                case "Mainstream":
                case "UKPDP":
                case "Comm":
                case "Croydon":
                case "Brit Ind":
                case "Humanity":
                case "Apni":
                case "EP":
                case "Nat Lib":
                case "NE Party":
                case "Beer BS":
                case "Young":
                case "Lincs Ind":
                case "Guildford":
                case "AWP":
                case "Comm Lge":
                case "Campaign":
                case "Ch P":
                case "U Party":
                case "Hospital":
                case "SEP":
                case "Hoi":
                case "S New":
                case "Digital":
                case "Green Soc":
                case "New IC":
                case "People Before Profit":
                case "Dem Ref":
                case "Plural":
                case "TSPP":
                case "Pirate":
                case "Real":
                case "Consensus":
                case "AD":
                case "Restore":
                case "Thanet":
                case "Communist":
                case "Poole":
                case "JACP":
                case "Roman":
                case "IPAP":
                case "IZB":
                case "Rochdale":
                case "Uttlesford":
                case "Reality":
                case "Atom":
                case "DP":
                case "Active Dem":
                case "Zeb":
                case "Manston":
                case "CSP":
                case "Southport":
                case "IASI":
                case "Ubuntu":
                case "PP UK":
                case "FPT":
                case "PPP":
                case "Magna Carta":
                case "Eccentric":
                case "Realist":
                case "Birthday":
                case "WVPTFP":
                case "Wigan":
                case "VAT":
                case "LP":
                case "Wessex Reg":
                case "Elmo":
                case "TEP":
                case "Ind CHC":
                case "SSP":
                case "SCP":
                case "Scottish CP":
                case "Soc Lab":
                case "PF":
                case "Change":
                case "ISWSL":
                case "Worth":
                case "Other":
                    return Party.Other;
                default:
                    throw new ArgumentException("Unknown party");
            }
        }

        private static Region GetRegionFromString(string region)
        {
            switch (region)
            {
                case "South East":
                case "South West":
                    return Region.South;

                case "North East":
                case "North West":
                case "Yorkshire and The Humber":
                    return Region.North;

                case "Scotland":
                    return Region.Scotland;

                case "Wales":
                case "East Midlands":
                case "West Midlands":
                case "East of England":
                    return Region.MidlandsWales;

                case "London":
                    return Region.London;

                default:
                    throw new ArgumentException("Unknown region");
            }
        }
    }
}
