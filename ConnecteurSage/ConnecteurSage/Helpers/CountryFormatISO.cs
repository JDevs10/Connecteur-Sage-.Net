using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Helpers
{
    public class CountryFormatISO
    {
        public CountryFormatISO()
        {

        }

        public string[,] getAllStaticCountryISOCode()
        {
            return new string[,] {
                { "Afghanistan", "AFG" },
                { "Îles Åland", "ALA" },
                { "Albanie", "ALB" },
                { "Algérie", "DZA" },
                { "Samoa américaines", "ASM" },
                { "Andorre", "AND" },
                { "Angola", "AGO" },
                { "Anguilla", "AIA" },
                { "Antarctique", "ATA" },
                { "Antigua-et-Barbuda", "ATG" },
                { "Argentine", "ARG" },
                { "Arménie", "ARM" },
                { "Aruba", "ABW" },
                { "Australie", "AUS" },
                { "Autriche", "AUT" },
                { "Azerbaïdjan", "AZE" },
                { "Bahamas", "BHS" },
                { "Bahreïn", "BHR" },
                { "Bangladesh", "BGD" },
                { "Barbade", "BRB" },
                { "Biélorussie", "BLR" },
                { "Belgique", "BEL" },
                { "Belize", "BLZ" },
                { "Bénin", "BEN" },
                { "Bermudes", "BMU" },
                { "Bhoutan", "BTN" },
                { "Bolivie", "BOL" },
                { "Bosnie-Herzégovine", "BIH" },
                { "Botswana", "BWA" },
                { "Île Bouvet", "BVT" },
                { "Brésil", "BRA" },
                { "British Virgin Islands", "VGB" },
                { "Territoire britannique de l’Océan Indien", "IOT" },
                { "Brunei Darussalam", "BRN" },
                { "Bulgarie", "BGR" },
                { "Burkina Faso", "BFA" },
                { "Burundi", "BDI" },
                { "Cambodge", "KHM" },
                { "Cameroun", "CMR" },
                { "Canada", "CAN" },
                { "Cap-Vert", "CPV" },
                { "Iles Cayman", "CYM" },
                { "République centrafricaine", "CAF" },
                { "Tchad", "TCD" },
                { "Chili", "CHL" },
                { "Chine", "CHN" },
                { "Hong Kong", "HKG" },
                { "Macao", "MAC" },
                { "Île Christmas", "CXR" },
                { "Îles Cocos", "CCK" },
                { "Colombie", "COL" },
                { "Comores", "COM" },
                { "République du Congo", "COG" },
                { "République démocratique du Congo", "COD" },
                { "Îles Cook", "COK" },
                { "Costa Rica", "CRI" },
                { "Côte d’Ivoire", "CIV" },
                { "Croatie", "HRV" },
                { "Cuba", "CUB" },
                { "Chypre", "CYP" },
                { "République tchèque", "CZE" },
                { "Danemark", "DNK" },
                { "Djibouti", "DJI" },
                { "Dominique", "DMA" },
                { "République dominicaine", "DOM" },
                { "Équateur", "ECU" },
                { "Égypte", "EGY" },
                { "Salvador", "SLV" },
                { "Guinée équatoriale", "GNQ" },
                { "Érythrée", "ERI" },
                { "Estonie", "EST" },
                { "Éthiopie", "ETH" },
                { "Îles Falkland", "FLK" },
                { "Îles Féroé", "FRO" },
                { "Fidji", "FJI" },
                { "Finlande", "FIN" },
                { "France", "FRA" },
                { "Guyane française", "GUF" },
                { "Polynésie française", "PYF" },
                { "Terres australes et antarctiques françaises", "ATF" },
                { "Gabon", "GAB" },
                { "Gambie", "GMB" },
                { "Géorgie", "GEO" },
                { "Allemagne", "DEU" },
                { "Ghana", "GHA" },
                { "Gibraltar", "GIB" },
                { "Grèce", "GRC" },
                { "Groenland", "GRL" },
                { "Grenade", "GRD" },
                { "Guadeloupe", "GLP" },
                { "Guam", "GUM" },
                { "Guatemala", "GTM" },
                { "Guernesey", "GGY" },
                { "Guinée", "GIN" },
                { "Guinée-Bissau", "GNB" },
                { "Guyane", "GUY" },
                { "Haïti", "HTI" },
                { "Îles Heard-et-MacDonald", "HMD" },
                { "Saint-Siège (Vatican)", "VAT" },
                { "Honduras", "HND" },
                { "Hongrie", "HUN" },
                { "Islande", "ISL" },
                { "Inde", "IND" },
                { "Indonésie", "IDN" },
                { "Iran", "IRN" },
                { "Irak", "IRQ" },
                { "Irlande", "IRL" },
                { "Ile de Man", "IMN" },
                { "Israël", "ISR" },
                { "Italie", "ITA" },
                { "Jamaïque", "JAM" },
                { "Japon", "JPN" },
                { "Jersey", "JEY" },
                { "Jordanie", "JOR" },
                { "Kazakhstan", "KAZ" },
                { "Kenya", "KEN" },
                { "Kiribati", "KIR" },
                { "Corée du Nord", "PRK" },
                { "Corée du Sud", "KOR" },
                { "Koweït", "KWT" },
                { "Kirghizistan", "KGZ" },
                { "Laos", "LAO" },
                { "Lettonie", "LVA" },
                { "Liban", "LBN" },
                { "Lesotho", "LSO" },
                { "Libéria", "LBR" },
                { "Libye", "LBY" },
                { "Liechtenstein", "LIE" },
                { "Lituanie", "LTU" },
                { "Luxembourg", "LUX" },
                { "Macédoine", "MKD" },
                { "Madagascar", "MDG" },
                { "Malawi", "MWI" },
                { "Malaisie", "MYS" },
                { "Maldives", "MDV" },
                { "Mali", "MLI" },
                { "Malte", "MLT" },
                { "Îles Marshall", "MHL" },
                { "Martinique", "MTQ" },
                { "Mauritanie", "MRT" },
                { "Maurice", "MUS" },
                { "Mayotte", "MYT" },
                { "Mexique", "MEX" },
                { "Micronésie", "FSM" },
                { "Moldavie", "MDA" },
                { "Monaco", "MCO" },
                { "Mongolie", "MNG" },
                { "Monténégro", "MNE" },
                { "Montserrat", "MSR" },
                { "Maroc", "MAR" },
                { "Mozambique", "MOZ" },
                { "Myanmar", "MMR" },
                { "Namibie", "NAM" },
                { "Nauru", "NRU" },
                { "Népal", "NPL" },
                { "Pays-Bas", "NLD" },
                { "Nouvelle-Calédonie", "NCL" },
                { "Nouvelle-Zélande", "NZL" },
                { "Nicaragua", "NIC" },
                { "Niger", "NER" },
                { "Nigeria", "NGA" },
                { "Niue", "NIU" },
                { "Île Norfolk", "NFK" },
                { "Îles Mariannes du Nord", "MNP" },
                { "Norvège", "NOR" },
                { "Oman", "OMN" },
                { "Pakistan", "PAK" },
                { "Palau", "PLW" },
                { "Palestine", "PSE" },
                { "Panama", "PAN" },
                { "Papouasie-Nouvelle-Guinée", "PNG" },
                { "Paraguay", "PRY" },
                { "Pérou", "PER" },
                { "Philippines", "PHL" },
                { "Pitcairn", "PCN" },
                { "Pologne", "POL" },
                { "Portugal", "PRT" },
                { "Puerto Rico", "PRI" },
                { "Qatar", "QAT" },
                { "Réunion", "REU" },
                { "Roumanie", "ROU" },
                { "Russie", "RUS" },
                { "Rwanda", "RWA" },
                { "Saint-Barthélemy", "BLM" },
                { "Sainte-Hélène", "SHN" },
                { "Saint-Kitts-et-Nevis", "KNA" },
                { "Sainte-Lucie", "LCA" },
                { "Saint-Martin (partie française)", "MAF" },
                { "Saint-Martin (partie néerlandaise)", "SXM" },
                { "Saint-Pierre-et-Miquelon", "SPM" },
                { "Saint-Vincent-et-les Grenadines", "VCT" },
                { "Samoa", "WSM" },
                { "Saint-Marin", "SMR" },
                { "Sao Tomé-et-Principe", "STP" },
                { "Arabie Saoudite", "SAU" },
                { "Sénégal", "SEN" },
                { "Serbie", "SRB" },
                { "Seychelles", "SYC" },
                { "Sierra Leone", "SLE" },
                { "Singapour", "SGP" },
                { "Slovaquie", "SVK" },
                { "Slovénie", "SVN" },
                { "Îles Salomon", "SLB" },
                { "Somalie", "SOM" },
                { "Afrique du Sud", "ZAF" },
                { "Géorgie du Sud et les îles Sandwich du Sud", "SGS" },
                { "Sud-Soudan", "SSD" },
                { "Espagne", "ESP" },
                { "Sri Lanka", "LKA" },
                { "Soudan", "SDN" },
                { "Suriname", "SUR" },
                { "Svalbard et Jan Mayen", "SJM" },
                { "Eswatini", "SWZ" },
                { "Suède", "SWE" },
                { "Suisse", "CHE" },
                { "Syrie", "SYR" },
                { "Taiwan", "TWN" },
                { "Tadjikistan", "TJK" },
                { "Tanzanie", "TZA" },
                { "Thaïlande", "THA" },
                { "Timor-Leste", "TLS" },
                { "Togo", "TGO" },
                { "Tokelau", "TKL" },
                { "Tonga", "TON" },
                { "Trinité-et-Tobago", "TTO" },
                { "Tunisie", "TUN" },
                { "Turquie", "TUR" },
                { "Turkménistan", "TKM" },
                { "Îles Turques-et-Caïques", "TCA" },
                { "Tuvalu", "TUV" },
                { "Ouganda", "UGA" },
                { "Ukraine", "UKR" },
                { "Émirats Arabes Unis", "ARE" },
                { "Royaume-Uni", "GBR" },
                { "États-Unis", "USA" },
                { "Îles mineures éloignées des États-Unis", "UMI" },
                { "Uruguay", "URY" },
                { "Ouzbékistan", "UZB" },
                { "Vanuatu", "VUT" },
                { "Venezuela", "VEN" },
                { "Viêt Nam", "VNM" },
                { "Îles Vierges américaines", "VIR" },
                { "Wallis-et-Futuna", "WLF" },
                { "Sahara occidental", "ESH" },
                { "Yémen", "YEM" },
                { "Zambie", "ZMB" },
                { "Zimbabwe", "ZWE" }
            };
        }

        public List<string> GetCountriesByName()
        {
            List<string> countries = new List<string>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                countries.Add("DisplayName: " + culture.DisplayName + " | ISO: " + convertOppositeCase(new StringBuilder(culture.ThreeLetterISOLanguageName)));
            }
            return countries;
        }

        public static List<RegionInfo> GetCountriesByIso3166()
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.Where(p => p.Name == country.Name).Count() == 0)
                    countries.Add(country);
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        public static List<RegionInfo> GetCountriesByCode(List<string> codes)
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            if (codes != null && codes.Count > 0)
            {
                foreach (string code in codes)
                {
                    try
                    {
                        countries.Add(new RegionInfo(code));
                    }
                    catch
                    {
                        //  Ignores the invalid culture code.
                    }
                }
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        // Method to convert characters  
        // of a string to opposite case 
        public string convertOppositeCase(StringBuilder str)
        {
            string result = "";
            int ln = str.Length;

            // Conversion according to ASCII values 
            for (int i = 0; i < ln; i++)
            {
                if (str[i] >= 'a' && str[i] <= 'z')
                {
                    //Convert lowercase to uppercase 
                    result += (char)(str[i] - 32) + "";
                }
                else if (str[i] >= 'A' && str[i] <= 'Z')
                {
                    //Convert uppercase to lowercase 
                    result += (char)(str[i] + 32) + "";
                }
            }
            return result;
        }
    }
}
