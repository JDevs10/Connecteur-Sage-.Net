using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using importPlanifier.Classes;

namespace importPlanifier.Helpers
{
    /// <summary>
    /// Classe statique permettant de stocker toutes les requêtes SQL utilisées dans l'application
    /// </summary>
    public static class QueryHelper
    {
        public static string getPrefix()
        {
            string result = "";
            string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (File.Exists(pathModule + @"\SettingSQL.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingSQL.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                result = setting.Prefix + ".dbo.";
                file.Close();
            }

            return result;
        }

        #region SQL Queries

        public static string getClient(bool sqlConnexion, string id)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise,  N_Expedition, CT_Langue, CT_Facture, N_Period, N_CatTarif, CT_Taux02, N_CatCompta FROM "+getPrefix()+ "F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=0";
            }
            else
            {
                return "SELECT CT_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise,  N_Expedition, CT_Langue, CT_Facture, N_Period, N_CatTarif, CT_Taux02, N_CatCompta FROM F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=0";
            }
            
        }

        public static string getDevise(bool sqlConnexion, string codeIso)
        {
            if (sqlConnexion)
            {
                return "select CBINDICE from "+getPrefix()+"P_DEVISE where D_CODEISO='" + codeIso + "'";
            }
            else
            {
                return "select CBINDICE from P_DEVISE where D_CODEISO='" + codeIso + "'";
            }
        }

        public static string fournisseurExiste(bool sqlConnexion, string id)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_Num FROM "+getPrefix()+ "F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=1";
            }
            else
            {
                return "SELECT CT_Num FROM F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=1";
            }
        }

        public static string getArticle(bool sqlConnexion, string id)
        {
            if (sqlConnexion)
            {
                return "SELECT AR_REF,AR_SuiviStock,AR_Gamme1,AR_Gamme2,AR_Nomencl,RP_CODEDEFAUT,AR_PRIXVEN,AR_POIDSBRUT,AR_POIDSNET,AR_UnitePoids,AR_DESIGN from "+getPrefix()+"F_ARTICLE where AR_CODEBARRE='" + id + "'";
            }
            else 
            { 
                return "SELECT AR_REF,AR_SuiviStock,AR_Gamme1,AR_Gamme2,AR_Nomencl,RP_CODEDEFAUT,AR_PRIXVEN,AR_POIDSBRUT,AR_POIDSNET,AR_UnitePoids,AR_DESIGN from F_ARTICLE where AR_CODEBARRE='" + id + "'";
            }
        }

        public static string getConditionnementArticle(bool sqlConnexion, string refArt)
        {
            if (sqlConnexion)
            {
                return "select EC_ENUMERE,EC_QUANTITE FROM "+getPrefix()+"F_CONDITION where CO_Principal = 1 and AR_REF='" + refArt + "'";
            }
            else
            {
                return "select EC_ENUMERE,EC_QUANTITE FROM F_CONDITION where CO_Principal = 1 and AR_REF='" + refArt + "'";
            }
        }

        public static string getStockId(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "select DE_NO FROM "+getPrefix()+"F_Depot where DE_Principal = 1";
            }
            else
            {
                return "select DE_NO FROM F_Depot where DE_Principal = 1";
            }
        }

        public static string getNumLivraison(bool sqlConnexion, string ct_num)
        {
            if (sqlConnexion)
            {
                return "select LI_NO FROM "+getPrefix()+"F_LIVRAISON where CT_NUM = '" + ct_num + "' and li_principal=1";
            }
            else
            {
                return "select LI_NO FROM F_LIVRAISON where CT_NUM = '" + ct_num + "' and li_principal=1";
            }
        }

        public static string deleteCommande(bool sqlConnexion, string numCommande)
        {
            if (sqlConnexion)
            {
                return "delete from "+getPrefix()+"F_docentete where do_piece='" + numCommande + "'";
            }
            else
            {
                return "delete from F_docentete where do_piece='" + numCommande + "'";
            }
        }

        public static string getGAMME(bool sqlConnexion, int type, string REF_Article)
        {
            if (sqlConnexion)
            {
                return "select AG_NO from "+getPrefix()+"F_artgamme where AG_TYPE=" + type + " and AR_REF='" + REF_Article + "'";
            }
            else
            {
                return "select AG_NO from F_artgamme where AG_TYPE=" + type + " and AR_REF='" + REF_Article + "'";
            }
        }

        public static string TestSiNumPieceExisteDeja(bool sqlConnexion, string num)
        {
            if (sqlConnexion)
            {
                return "select do_piece from "+getPrefix()+"f_docentete where do_piece='" + num + "'";
            }
            else
            {
                return "select do_piece from f_docentete where do_piece='" + num + "'";
            }
        }

        public static string TestIntituleLivraison(bool sqlConnexion, string LI_intitule)
        {
            if (sqlConnexion)
            {
                return "select LI_intitule FROM "+getPrefix()+"F_LIVRAISON where LI_intitule like '" + LI_intitule + "%'";
            }
            else
            {
                return "select LI_intitule FROM F_LIVRAISON where LI_intitule like '" + LI_intitule + "%'";
            }
        }

        public static string insertCommande(bool sqlConnexion, Client client, Order order)
        {
            if (sqlConnexion)
            {
                return "Insert into " +
                    "" + getPrefix() + "F_DOCENTETE(CG_NUM,CT_NUMPAYEUR,DE_NO,DO_ATTENTE,DO_BLFACT,DO_CLOTURE,DO_COLISAGE," +
                    "DO_CONDITION,DO_DATE,DO_DATELIVR,DO_DEVISE,DO_DOMAINE," +
                    "DO_EXPEDIT,DO_LANGUE,DO_NBFACTURE,DO_PERIOD,DO_PIECE,DO_REF," +
                    "DO_REGIME,DO_STATUT,DO_TARIF,DO_TIERS,DO_TRANSACTION," +
                    "DO_TXESCOMPTE,DO_TYPE,DO_TYPECOLIS,DO_VENTILE," +
                    "LI_NO,N_CATCOMPTA,DO_MOTIF,DO_COORD01,DO_TAXE1,DO_TypeTaux1,DO_TypeTaxe1) " +
                    "values" +
                    "('" + client.CG_NumPrinc + "','" + client.CT_NumPayeur + "'," + order.StockId + ",0,0,0,1," +
                    "" + order.conditionLivraison + ",'{d " + order.DateCommande + "}'," + order.DateLivraison + "," + client.N_Devise + ",0," +
                    "" + client.N_Expedition + "," + client.CT_Langue + "," + client.CT_Facture + "," + client.N_Period + ",'" + order.Id + "','" + order.Reference + "'," +
                    "21,2," + client.N_CatTarif + ",'" + client.CT_Num + "',11," +
                    "" + client.CT_Taux02 + ",1,1,0," +
                    "" + order.adresseLivraison + "," + client.N_CatCompta + ",'" + order.codeAcheteur + ";" + order.codeFournisseur + "','" + order.NumCommande + "',20,0,0)";
            }
            else
            {
                return "Insert into " +
                    "F_DOCENTETE(CG_NUM,CT_NUMPAYEUR,DE_NO,DO_ATTENTE,DO_BLFACT,DO_CLOTURE,DO_COLISAGE," +
                    "DO_CONDITION,DO_DATE,DO_DATELIVR,DO_DEVISE,DO_DOMAINE," +
                    "DO_EXPEDIT,DO_LANGUE,DO_NBFACTURE,DO_PERIOD,DO_PIECE,DO_REF," +
                    "DO_REGIME,DO_STATUT,DO_TARIF,DO_TIERS,DO_TRANSACTION," +
                    "DO_TXESCOMPTE,DO_TYPE,DO_TYPECOLIS,DO_VENTILE," +
                    "LI_NO,N_CATCOMPTA,DO_MOTIF,DO_COORD01,DO_TAXE1,DO_TypeTaux1,DO_TypeTaxe1) " +
                    "values" +
                    "('" + client.CG_NumPrinc + "','" + client.CT_NumPayeur + "'," + order.StockId + ",0,0,0,1," +
                    "" + order.conditionLivraison + ",'{d " + order.DateCommande + "}'," + order.DateLivraison + "," + client.N_Devise + ",0," +
                    "" + client.N_Expedition + "," + client.CT_Langue + "," + client.CT_Facture + "," + client.N_Period + ",'" + order.Id + "','" + order.Reference + "'," +
                    "21,2," + client.N_CatTarif + ",'" + client.CT_Num + "',11," +
                    "" + client.CT_Taux02 + ",1,1,0," +
                    "" + order.adresseLivraison + "," + client.N_CatCompta + ",'" + order.codeAcheteur + ";" + order.codeFournisseur + "','" + order.NumCommande + "',20,0,0)";
            }
        }

        public static string insertLigneCommande(bool sqlConnexion, Client client, Order order, OrderLine line)
        {
            if (sqlConnexion)
            {
                return "Insert Into " +
             "" + getPrefix() + "F_DOCLIGNE(AC_REFCLIENT,AF_REFFOURNISS,AR_REF,CT_NUM,DE_NO," +
             "DL_DATEBC,DL_DESIGN,DL_LIGNE,DL_PRIXUNITAIRE,DL_QTE,EU_Enumere,EU_QTE," +
             "DL_VALORISE,DO_DATE,DO_DATELIVR,DO_DOMAINE," +
             "DO_PIECE,DO_TYPE,AG_No1,AG_No2,AR_RefCompose,RP_Code,do_ref,DL_PoidsNet,DL_PoidsBrut,DL_Taxe1,DL_TypeTaux1,DL_TypeTaxe1) " +
             "values " +
             "('" + line.codeAcheteur + "','" + line.codeFournis + "','" + line.article.AR_REF + "','" + client.CT_Num + "'," + line.article.AR_StockId + ", " +
             "'{d " + order.DateCommande + "}','" + line.descriptionArticle + "'," + line.NumLigne + "," + line.PrixNetHT + "," + line.Quantite + ",'" + (line.article.Conditionnement != null ? line.article.Conditionnement.EC_ENUMERE : "") + "'," + (line.Calcule_conditionnement != "0" ? line.Calcule_conditionnement : line.Quantite) + "," +
             "1,'{d " + order.DateCommande + "}'," + line.DateLivraison + ",0,'" + order.Id + "',1," + line.article.gamme1 + "," + line.article.gamme2 + ",'" + line.article.AR_REFCompose + "','" + line.article.RP_CODEDEFAUT + "','" + line.codeFournis + "'," + line.article.AR_POIDSNET.Replace(",", ".") + "," + line.article.AR_POIDSBRUT.Replace(",", ".") + ",20,0,0)";
            }
            else
            {
                return "Insert Into " +
             "F_DOCLIGNE(AC_REFCLIENT,AF_REFFOURNISS,AR_REF,CT_NUM,DE_NO," +
             "DL_DATEBC,DL_DESIGN,DL_LIGNE,DL_PRIXUNITAIRE,DL_QTE,EU_Enumere,EU_QTE," +
             "DL_VALORISE,DO_DATE,DO_DATELIVR,DO_DOMAINE," +
             "DO_PIECE,DO_TYPE,AG_No1,AG_No2,AR_RefCompose,RP_Code,do_ref,DL_PoidsNet,DL_PoidsBrut,DL_Taxe1,DL_TypeTaux1,DL_TypeTaxe1) " +
             "values " +
             "('" + line.codeAcheteur + "','" + line.codeFournis + "','" + line.article.AR_REF + "','" + client.CT_Num + "'," + line.article.AR_StockId + ", " +
             "'{d " + order.DateCommande + "}','" + line.descriptionArticle + "'," + line.NumLigne + "," + line.PrixNetHT + "," + line.Quantite + ",'" + (line.article.Conditionnement != null ? line.article.Conditionnement.EC_ENUMERE : "") + "'," + (line.Calcule_conditionnement != "0" ? line.Calcule_conditionnement : line.Quantite) + "," +
             "1,'{d " + order.DateCommande + "}'," + line.DateLivraison + ",0,'" + order.Id + "',1," + line.article.gamme1 + "," + line.article.gamme2 + ",'" + line.article.AR_REFCompose + "','" + line.article.RP_CODEDEFAUT + "','" + line.codeFournis + "'," + line.article.AR_POIDSNET.Replace(",", ".") + "," + line.article.AR_POIDSBRUT.Replace(",", ".") + ",20,0,0)";
            }
        }

        public static string getInfoSociete(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT D_RaisonSoc,D_Adresse,D_CodePostal,D_Ville,D_Pays,D_Siret,D_Identifiant,D_Commentaire from " + getPrefix() + "p_dossier";
            }
            else
            {
                return "SELECT D_RaisonSoc,D_Adresse,D_CodePostal,D_Ville,D_Pays,D_Siret,D_Identifiant,D_Commentaire from p_dossier";
            }
        }

        public static string getGNLClientLivraison(bool sqlConnexion, string intitule)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_EdiCode from " + getPrefix()+"f_comptet where CT_intitule='" + intitule + "'";
            }
            else
            {
                return "SELECT CT_EdiCode from f_comptet where CT_intitule='" + intitule + "'";
            }
        }

        public static string get_last_Num_Livraison(bool sqlConnexion, string client)
        {
            if (sqlConnexion)
            {
                return "select max(LI_NO) FROM "+getPrefix()+"F_LIVRAISON WHERE CT_NUM='" + client + "'";
            }
            else
            {
                return "select max(LI_NO) FROM F_LIVRAISON WHERE CT_NUM='" + client + "'";
            }
        }

        public static string MaxNumPiece(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT Max(F_DOCENTETE.DO_PIECE) FROM " + getPrefix() + "F_DOCENTETE F_DOCENTETE WHERE (F_DOCENTETE.DO_DOMAINE=0) AND (F_DOCENTETE.DO_TYPE=1)";
            }
            else 
            { 
                return "SELECT Max(F_DOCENTETE.DO_PIECE) FROM F_DOCENTETE F_DOCENTETE WHERE (F_DOCENTETE.DO_DOMAINE=0) AND (F_DOCENTETE.DO_TYPE=1)"; 
            }
        }

        public static string get_Next_NumPiece_BonCommande(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT DC_PIECE FROM " + getPrefix() + "F_DOCCURRENTPIECE WHERE DC_IDCOL=1 and DC_SOUCHE=0";
            }
            else
            {
                return "SELECT DC_PIECE FROM F_DOCCURRENTPIECE WHERE DC_IDCOL=1 and DC_SOUCHE=0";
            }
        }

        public static string get_condition_livraison_indice(bool sqlConnexion, string c_mode)
        {
            if (sqlConnexion)
            {
                return "SELECT p_Condlivr.CBINDICE FROM "+getPrefix()+"p_Condlivr p_Condlivr WHERE (p_Condlivr.C_MODE Like '" + c_mode + "%')";
            }
            else
            {
                return "SELECT p_Condlivr.CBINDICE FROM p_Condlivr p_Condlivr WHERE (p_Condlivr.C_MODE Like '" + c_mode + "%')";
            }
        }

        public static string get_condition_livraison_mode(bool sqlConnexion, string indice)
        {
            if (sqlConnexion)
            {
                return "SELECT C_MODE FROM "+getPrefix()+"p_Condlivr WHERE CBINDICE=" + indice + "";
            }
            else
            {
                return "SELECT C_MODE FROM p_Condlivr WHERE CBINDICE=" + indice + "";
            }
        }

        public static string get_adresse_livraison(bool sqlConnexion, AdresseLivraison adresse)
        {
            if (sqlConnexion)
            {
                return "Select li_no,li_contact,Li_adresse,li_codepostal,li_ville,li_pays from " + getPrefix() + "f_livraison where li_pays='" + adresse.pays + "' and li_codepostal='" + adresse.codePostale + "' and li_ville='" + adresse.ville + "' and Li_adresse='" + adresse.adresse + "' and li_contact='" + adresse.Nom_contact + "' and CT_NUM='" + adresse.CT_NUM + "'";
            }
            else
            {
                return "Select li_no,li_contact,Li_adresse,li_codepostal,li_ville,li_pays from f_livraison where li_pays='" + adresse.pays + "' and li_codepostal='" + adresse.codePostale + "' and li_ville='" + adresse.ville + "' and Li_adresse='" + adresse.adresse + "' and li_contact='" + adresse.Nom_contact + "' and CT_NUM='" + adresse.CT_NUM + "'";
            }
        }

        public static string get_NumPiece_Motif(bool sqlConnexion, string num)
        {
            if (sqlConnexion)
            {
                return "SELECT DO_PIECE FROM " + getPrefix() + "F_DOCENTETE WHERE DO_COORD01='" + num + "'";
            }
            else
            {
                return "SELECT DO_PIECE FROM F_DOCENTETE WHERE DO_COORD01='" + num + "'";
            }
        }

        public static string get_ForCorrect(bool sqlConnexion, string cmd)
        {
            if (sqlConnexion)
            {
                return "select doc.do_tiers,doc.LI_NO,liv.li_intitule,liv.li_contact,liv.li_adresse,liv.li_codepostale,liv.li_ville,liv.li_pays from " + getPrefix() + "f_docentete doc, " + getPrefix() + "f_livraison liv where doc.DO_COORD01='" + cmd + "' and doc.li_no=liv.li_no";
            }
            else
            {
                return "select doc.do_tiers,doc.LI_NO,liv.li_intitule,liv.li_contact,liv.li_adresse,liv.li_codepostale,liv.li_ville,liv.li_pays from f_docentete doc,f_livraison liv where doc.DO_COORD01='" + cmd + "' and doc.li_no=liv.li_no";
            }
        }

        public static string insert_adresse_livraison(bool sqlConnexion, string client, AdresseLivraison adresse)
        {
            if (sqlConnexion)
            {
                //var intitule = string.Format(adresse.ville + "_{0:HHmmss}", DateTime.Now);
                if (adresse.pays == "FR")
                {
                    adresse.pays = "FRANCE";
                }

                return "Insert Into " + getPrefix() + "F_LIVRAISON (CT_Num, LI_Intitule, N_Condition, N_Expedition, " +
                     "LI_Adresse, LI_CodePostal, LI_Ville, LI_Contact, LI_Pays) Values ('" + client + "', '" + adresse.intitule + "', " + adresse.condition + ", 1," +
                     "'" + adresse.adresse + "', '" + adresse.codePostale + "', '" + adresse.ville + "', '" + adresse.Nom_contact + "', '" + adresse.pays + "')";
            }
            else
            {
                //var intitule = string.Format(adresse.ville + "_{0:HHmmss}", DateTime.Now);
                if (adresse.pays == "FR")
                {
                    adresse.pays = "FRANCE";
                }

                return "Insert Into F_LIVRAISON (CT_Num, LI_Intitule, N_Condition, N_Expedition, " +
                     "LI_Adresse, LI_CodePostal, LI_Ville, LI_Contact, LI_Pays) Values ('" + client + "', '" + adresse.intitule + "', " + adresse.condition + ", 1," +
                     "'" + adresse.adresse + "', '" + adresse.codePostale + "', '" + adresse.ville + "', '" + adresse.Nom_contact + "', '" + adresse.pays + "')";
            }
        }


        // ******************************************************************************************

        //public static string getDevise(string codeIso)
        //{
        //    return "select CBINDICE from P_DEVISE where D_CODEISO='" + codeIso + "'";
        //}

        public static string getDeviseIso(bool sqlConnexion, string code)
        {
            if (sqlConnexion)
            {
                return "select D_CODEISO from "+getPrefix()+"P_DEVISE where CBINDICE=" + code + "";
            }
            else
            {
                return "select D_CODEISO from P_DEVISE where CBINDICE=" + code + "";
            }
        }

        public static string getListCommandes(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT doc.DO_PIECE, cli.CT_EdiCode, liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_COMPLEMENT, liv.LI_VILLE, liv.LI_PAYS, doc.DO_DEVISE, doc.DO_DATE, doc.DO_DATELIVR, cond.C_MODE, doc.FNT_TOTALHTNET,doc.do_tiers,doc.do_motif,doc.do_coord01,liv.li_contact " +
                 "FROM " + getPrefix() + "F_comptet cli, " + getPrefix() + "P_condlivr cond, " + getPrefix() + "F_docentete doc, " + getPrefix() + "F_LIVRAISON liv " +
                 "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=1) AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition) AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
            }
            else
            {
                return "SELECT doc.DO_PIECE, cli.CT_EdiCode, liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_COMPLEMENT, liv.LI_VILLE, liv.LI_PAYS, doc.DO_DEVISE, doc.DO_DATE, doc.DO_DATELIVR, cond.C_MODE, doc.FNT_TOTALHTNET,doc.do_tiers,doc.do_motif,doc.do_coord01,liv.li_contact " +
                 "FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv " +
                 "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=1) AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition) AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
            }
        }

        public static string getListLignesCommandes(bool sqlConnexion, string codeCommande)
        {
            if (sqlConnexion)
            {
                return "SELECT doc.DL_LIGNE, art.AR_CODEBARRE, doc.DL_DESIGN, doc.DL_QTE, doc.DL_PRIXUNITAIRE, doc.DL_MONTANTHT, doc.DO_DATELIVR, doc.do_ref, doc.AC_REFCLIENT " +
                 "FROM " + getPrefix() + "F_ARTICLE art, " + getPrefix() + "F_DOCLIGNE doc " +
                 "WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeCommande + "'";
            }
            else
            {
                return "SELECT doc.DL_LIGNE, art.AR_CODEBARRE, doc.DL_DESIGN, doc.DL_QTE, doc.DL_PRIXUNITAIRE, doc.DL_MONTANTHT, doc.DO_DATELIVR, doc.do_ref, doc.AC_REFCLIENT " +
                 "FROM F_ARTICLE art, F_DOCLIGNE doc " +
                 "WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeCommande + "'";
            }
        }


        // ******************************************************************************************

        public static string getListDocumentVente(bool sqlConnexion, int type)
        {
            if (sqlConnexion)
            {
                if (type == 67)
                {
                    return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                         "FROM " + getPrefix() + "F_comptet cli, " + getPrefix() + "P_condlivr cond, " + getPrefix() + "F_docentete doc, " + getPrefix() + "F_LIVRAISON liv " +
                         "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=5 or doc.DO_TYPE=6 or doc.DO_TYPE=7)  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
                }
                // J'ai modifier DO_COORD01 par DO_REF pour le client TRACE SPORT
                return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                    "FROM " + getPrefix() + "F_comptet cli, " + getPrefix() + "P_condlivr cond, " + getPrefix() + "F_docentete doc, " + getPrefix() + "F_LIVRAISON liv " +
                    "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=" + type + ")  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";

                /*
                if (type == 67)
                {
                    return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                         "FROM " + getPrefix() + "F_comptet cli, " + getPrefix() + "P_condlivr cond, " + getPrefix() + "F_docentete doc, " + getPrefix() + "F_LIVRAISON liv " +
                         "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=5 or doc.DO_TYPE=6 or doc.DO_TYPE=7)  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
                }
                // J'ai modifier DO_COORD01 par DO_REF pour le client TRACE SPORT
                return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                    "FROM " + getPrefix() + "F_comptet cli, " + getPrefix() + "P_condlivr cond, " + getPrefix() + "F_docentete doc, " + getPrefix() + "F_LIVRAISON liv " +
                    "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=" + type + ")  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
                    */
            }
            else
            {
                if (type == 67)
                {
                    return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                     "FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv " +
                     "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=5 or doc.DO_TYPE=6 or doc.DO_TYPE=7)  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
                }
                // J'ai modifier DO_COORD01 par DO_REF pour le client TRACE SPORT
                return "SELECT doc.DO_Piece,doc.DO_TIERS,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num  " +
                    "FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv " +
                    "WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=" + type + ")  AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers) and doc.DO_COORD02='1'";
            }
        }

        public static string getListDocumentVenteLine(bool sqlConnexion, string codeDocument)
        {
            if (sqlConnexion)
            {
                return "SELECT doc.DO_Date,doc.DO_DateLivr,doc.DL_Ligne,doc.AR_Ref,doc.DL_Design,doc.DL_Qte,doc.DL_QteBC,doc.DL_QteBL,doc.EU_Qte,doc.DL_PoidsNet,doc.DL_PoidsBrut,doc.DL_Remise01REM_Valeur,doc.DL_Remise01REM_Type,doc.DL_Remise03REM_Valeur,doc.DL_Remise03REM_Type,doc.DL_PrixUnitaire,doc.DL_Taxe1,doc.DL_Taxe2,doc.DL_Taxe3,doc.DL_TypeTaxe1,doc.DL_TypeTaxe2,doc.DL_TypeTaxe3,doc.DL_MontantHT,doc.DL_MontantTTC,doc.DL_NoColis,doc.FNT_MontantHT,doc.FNT_MontantTaxes,doc.FNT_MontantTTC,doc.FNT_PrixUNet,doc.FNT_PrixUNetTTC,doc.FNT_RemiseGlobale,art.AR_CODEBARRE " +
                    "FROM " + getPrefix() + "F_ARTICLE art, " + getPrefix() + "F_DOCLIGNE doc " +
                    "WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeDocument + "'";
            }
            else
            {
                return "SELECT doc.DO_Date,doc.DO_DateLivr,doc.DL_Ligne,doc.AR_Ref,doc.DL_Design,doc.DL_Qte,doc.DL_QteBC,doc.DL_QteBL,doc.EU_Qte,doc.DL_PoidsNet,doc.DL_PoidsBrut,doc.DL_Remise01REM_Valeur,doc.DL_Remise01REM_Type,doc.DL_Remise03REM_Valeur,doc.DL_Remise03REM_Type,doc.DL_PrixUnitaire,doc.DL_Taxe1,doc.DL_Taxe2,doc.DL_Taxe3,doc.DL_TypeTaxe1,doc.DL_TypeTaxe2,doc.DL_TypeTaxe3,doc.DL_MontantHT,doc.DL_MontantTTC,doc.DL_NoColis,doc.FNT_MontantHT,doc.FNT_MontantTaxes,doc.FNT_MontantTTC,doc.FNT_PrixUNet,doc.FNT_PrixUNetTTC,doc.FNT_RemiseGlobale,art.AR_CODEBARRE " +
                     "FROM F_ARTICLE art, F_DOCLIGNE doc " +
                     "WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeDocument + "'";
            }
        }

        public static string getCustomer(bool sqlConnexion, string do_tiers)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_Num,CT_Intitule,CT_Adresse,CT_APE,CT_CodePostal,CT_CodeRegion,CT_Complement,CT_CONTACT,CT_EdiCode,CT_email,CT_Identifiant, CT_Ville,CT_Pays,CT_Siret,CT_Telephone,N_DEVISE,CT_SvFormeJuri  FROM " + getPrefix() + "F_COMPTET where CT_Type=0 and CT_Num='" + do_tiers + "'";
            }
            else
            {
                return "SELECT CT_Num,CT_Intitule,CT_Adresse,CT_APE,CT_CodePostal,CT_CodeRegion,CT_Complement,CT_CONTACT,CT_EdiCode,CT_email,CT_Identifiant, CT_Ville,CT_Pays,CT_Siret,CT_Telephone,N_DEVISE,CT_SvFormeJuri  FROM F_COMPTET where CT_Type=0 and CT_Num='" + do_tiers + "'";
            }
        }

        public static string updateDocumentdeVente(bool sqlConnexion, string do_piece)
        {
            if (sqlConnexion)
            {
                return "UPDATE " + getPrefix() + "f_docentete set DO_COORD02='0' where do_piece='" + do_piece + "'";

            }
            else
            {
                return "UPDATE f_docentete set DO_COORD02='0' where do_piece='" + do_piece + "'";
            }
        }

        public static string getModeReglement(bool sqlConnexion, string do_piece)
        {
            if (sqlConnexion)
            {
                return "select int_reglement,DR_DATE,DR_TYPEREGL,DR_POURCENT,DR_MONTANT from " + getPrefix() + "f_docregl where do_piece='" + do_piece + "'";
            }
            else
            {
                return "select int_reglement,DR_DATE,DR_TYPEREGL,DR_POURCENT,DR_MONTANT from f_docregl where do_piece='" + do_piece + "'";
            }
        }


        public static string getCommandeStatut(bool sqlConnexion)
        {
            ConfigurationExport export = new ConfigurationExport();
            export.Load();

            if (sqlConnexion)
            {
                return "SELECT cbMarq, DO_Statut FROM " + getPrefix() + "F_DOCENTETE WHERE DO_Type = 1 AND DO_Statut = " + export.exportBonsCommandes_Statut + " ORDER BY cbMarq DESC";
            }
            else
            {
                return "SELECT cbMarq, DO_Statut FROM F_DOCENTETE WHERE DO_Type = 1 AND DO_Statut = " + export.exportBonsCommandes_Statut + " ORDER BY cbMarq DESC";
            }
        }

        public static string changeOrderStatut(bool sqlConnexion, string cmd_NumCommande)
        {
            if (sqlConnexion)
            {
                return "UPDATE " + getPrefix() + "F_DOCENTETE SET DO_Statut = 2 WHERE DO_PIECE = '" + cmd_NumCommande + "' ";
            }
            else
            {
                return "UPDATE F_DOCENTETE SET DO_Statut = 2 WHERE DO_PIECE = '" + cmd_NumCommande + "'";
            }
        }

        public static string getStockInfo(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock FROM " + getPrefix() + "F_ARTICLE as Art, " + getPrefix() + "F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
                // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM "+ getPrefix() + "BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
            }
            else
            {
                return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock FROM F_ARTICLE as Art, F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
                // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM "+ getPrefix() + "BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
            }
        }

        /* ============================================= VERSION 0.6 =============================================*/

        public static string getLastPieceNumberReference(bool sqlConnexion, string mask)
        {
            if (sqlConnexion)
            {
                if (mask == "BL")
                {
                    return "SELECT DO_Piece FROM " + getPrefix() + "F_DOCENTETE WHERE DO_Piece LIKE '" + mask + "%' AND DO_Type = 3 ORDER BY DO_Piece DESC";    //SQL For DESADV 'BL'
                }
                else
                {
                    return "SELECT DO_Piece FROM " + getPrefix() + "F_DOCENTETE WHERE DO_Piece LIKE '" + mask + "%' ORDER BY DO_Piece DESC";
                }
            }
            else
            {
                return "SELECT DO_Piece FROM F_DOCENTETE WHERE DO_Piece LIKE '" + mask + "%' ORDER BY DO_Piece DESC";
            }
        }

        public static string getProductNameByReference_DESADV(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                return "SELECT AR_Ref, AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch, AR_PrixVen FROM " + getPrefix() + "F_ARTICLE WHERE AR_Ref IN('" + reference + "') ";
            }
            else
            {
                return "SELECT AR_Ref, AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch, AR_PrixVen FROM F_ARTICLE WHERE AR_Ref IN('" + reference + "') ";
            }
        }

        public static string getClientReferenceFromCMD_DESADV(bool sqlConnexion, string reference_cmd)
        {
            if (sqlConnexion)
            {
                return "SELECT DO_TIERS FROM " + getPrefix() + "F_DOCENTETE WHERE DO_Piece IN('" + reference_cmd + "') ";
            }
            else
            {
                return "SELECT DO_TIERS FROM F_DOCENTETE WHERE DO_Piece IN('" + reference_cmd + "') ";
            }
        }

        public static string getClientReferenceById_DESADV(bool sqlConnexion, string CT_Num)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_Num, CA_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise, CT_Langue, CT_Facture, CT_Taux02, N_CatCompta, CO_No, N_CatTarif, N_Expedition FROM " + getPrefix() + "F_Comptet WHERE CT_Num IN('" + CT_Num + "')";
            }
            else
            {
                return "SELECT CT_Num, CA_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise, CT_Langue, CT_Facture, CT_Taux02, N_CatCompta, CO_No, N_CatTarif, N_Expedition FROM F_Comptet WHERE CT_Num IN('" + CT_Num + "')";
            }
        }

        public static string getNegativeStockOfAProduct(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
                //return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
                return "SELECT SUM(DL_Qte) FROM " + getPrefix() +"F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
            }
            else
            {
                //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
                //return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
                return "SELECT SUM(DL_Qte) FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
            }
        }

        public static string getPositiveStockOfAProduct(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
                //return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
                return "SELECT SUM(DL_Qte) FROM " + getPrefix() + "F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
            }
            else
            {
                //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
                //return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
                return "SELECT SUM(DL_Qte) FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
            }
        }

        public static string getProductNameByReference(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                return "SELECT AR_Design,AR_PoidsNet,AR_PoidsBrut,AR_PrixAch FROM "+getPrefix()+"F_ARTICLE WHERE AR_Ref IN('" + reference + "') ";
            }
            else
            {
                return "SELECT AR_Design,AR_PoidsNet,AR_PoidsBrut,AR_PrixAch FROM F_ARTICLE WHERE AR_Ref IN('" + reference + "') ";
            }
        }

        public static string insertStockDocument(bool sqlConnexion, string DO_Type, string reference_doc, string curr_date, string DO_REF, string curr_date_time)
        {
            if (sqlConnexion)
            {
                string sql = "INSERT INTO " + getPrefix() + "F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, COMMENTAIRES)" +
                             "VALUES (NULL, NULL, 0, 0, 0, 0, 1, 0,{d '" + curr_date + "'}, NULL, 0, 2, 0 , 0, 0 , 0, '" + reference_doc + "', '" + DO_REF + "', 0, 0 , 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '','" + reference_doc + ":document from logistic.')";
                return sql;
            }
            else
            {
                string sql = "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, COMMENTAIRES)" +
                             "VALUES (NULL, NULL, 0, 0, 0, 0, 1, 0,{d '" + curr_date + "'}, NULL, 0, 2, 0 , 0, 0 , 0, '" + reference_doc + "', '" + DO_REF + "', 0, 0 , 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '','" + reference_doc + ":document from logistic.')";
                return sql;
            }
        }

        public static string insertStockDocumentLine(bool sqlConnexion, string[,] products, int x)
        {
            if (sqlConnexion)
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT INTO " + getPrefix() + "F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) " +
                             "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ")";
                return sql;
            }
            else
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) " +
                             "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ")";
                return sql;
            }
        }

        public static string insertVeologStockDocument(bool sqlConnexion, string DO_Type, string reference_doc, string curr_date, string DO_REF, string DO_TotalHT, string DO_TotalTTC, string veologImportDate)
        {
            if (sqlConnexion)
            {
                string sql = "INSERT INTO " + getPrefix() + "F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, COMMENTAIRES, DO_COURS, Nature_OP_P, DO_TotalHT, DO_TotalHTNet, DO_TotalTTC, DO_NetAPayer, DO_MontantRegle, Veolog)" +
                             "VALUES (NULL, NULL, 0, 0, 0, 0, 1, 0, {d '" + curr_date + "'}, NULL, 0, 2, 0, 0, 0, 0, '" + reference_doc + "', '" + DO_REF + "', 0, 0, 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '', '" + reference_doc + ":document from logistic.', 0.000000, '', " + DO_TotalHT + ", 0.000000, " + DO_TotalTTC + ", 0.000000, 0.000000, '"+ veologImportDate + "')";
                return sql;
            }
            else
            {
                string sql = "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, COMMENTAIRES)" +
                             "VALUES (NULL, NULL, 0, 0, 0, 0, 1, 0,{d '" + curr_date + "'}, NULL, 0, 2, 0 , 0, 0 , 0, '" + reference_doc + "', '" + DO_REF + "', 0, 0 , 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '','" + reference_doc + ":document from logistic.')";
                return sql;
            }
        }

        public static string insertVeologStockDocumentLine(bool sqlConnexion, string[,] products, int x)
        {
            if (sqlConnexion)
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT INTO " + getPrefix() + "F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte, DL_PUTTC, DL_TTC, DL_PieceBC, DL_PieceBL, DL_DateBL, DL_TNomencl, DL_TRemPied, DL_TRemExep, DL_QteBC, DL_QteBL, DL_Remise01REM_Valeur, DL_Remise01REM_Type, DL_Remise02REM_Valeur, DL_Remise02REM_Type, DL_Remise03REM_Valeur, DL_Remise03REM_Type, DL_NoRef, DL_TypePL, DL_PUDevise, CA_Num, DL_Frais, AC_RefClient, DL_PiecePL, DL_DatePL, DL_QtePL, DL_NoColis, DL_NoLink, CO_No, DT_No, DL_PieceDE, DL_DateDe, DL_QteDE, DL_NoSousTotal, CA_No, DL_PUBC, DL_CodeTaxe1, DL_Taxe1, DL_Taxe2, DL_Taxe3, DL_TypeTaux1, DL_TypeTaxe1, DL_TypeTaux2, DL_TypeTaxe2, DL_TypeTaux3, DL_TypeTaxe3) " +
                             "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ", " + products[x, 27] + ", " + products[x, 28] + ", '" + products[x, 29] + "', '" + products[x, 30] + "', {d '" + products[x, 31] + "'}, " + products[x, 32] + ", " + products[x, 33] + ", " + products[x, 34] + ", " + products[x, 35] + ", " + products[x, 36] + ", " + products[x, 37] + ", " + products[x, 38] + ", " + products[x, 39] + ", " + products[x, 40] + ", " + products[x, 41] + ", " + products[x, 42] + ", " + products[x, 43] + ", " + products[x, 44] + ", " + products[x, 45] + ", '" + products[x, 46] + "', " + products[x, 47] + ", '" + products[x, 48] + "', " + products[x, 49] + ", {d '" + products[x, 50] + "'}, " + products[x, 51] + ", '" + products[x, 52] + "', " + products[x, 53] + ", " + products[x, 54] + ", " + products[x, 55] + ", '" + products[x, 56] + "', {d '" + products[x, 57] + "'}, " + products[x, 58] + ", " + products[x, 59] + ", " + products[x, 60] + ", " + products[x, 61] + ", '" + products[x, 62] + "', " + products[x, 63] + ", " + products[x, 64] + ", " + products[x, 65] + ", " + products[x, 66] + ", " + products[x, 67] + ", " + products[x, 68] + ", " + products[x, 69] + ", " + products[x, 70] + ", " + products[x, 71] + ")";
                return sql;
            }
            else
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) " +
                             "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ")";
                return sql;
            }
        }

        public static string updateVeologDeliveryDate(bool sqlConnexion, string reference, string date)
        {
            if (sqlConnexion)
            {
                return "UPDATE " + getPrefix() + "F_DOCENTETE SET Veolog = '"+ date + "' WHERE DO_Piece = '"+ reference + "'";
            }
            else
            {
                return "UPDATE F_DOCENTETE SET Veolog = '" + date + "' WHERE DO_Piece = '" + reference + "'";
            }
        }

        public static string getCoommandeById(bool sqlConnexion, string cbMarq)
        {
            if (sqlConnexion)
            {
                return "SELECT DISTINCT(DO_Piece)as DO_Piece, cli.CT_Num, CONCAT(cli.CT_Complement,',',cli.CT_CodePostal,',',cli.CT_Ville, ',',cli.CT_Pays)as Adresse, cmd.DO_DEVISE, cmd.DO_Date, cmd.DO_DateLivr, cmd.DO_Condition, cmd.DO_TotalHT, cli.CT_Intitule, cmd.DO_Motif, cli.CT_EdiCode, cmd.N_CATCOMPTA, liv.LI_Contact, cli.CT_Telephone, cli.CT_EMail " +
                    "FROM " + getPrefix() + "F_COMPTET as cli, " + getPrefix() + "F_DOCENTETE as cmd, " + getPrefix() + "F_livraison liv " +
                    "WHERE cmd.DO_Tiers = cli.CT_Num AND cmd.DO_Tiers = liv.CT_Num " +
                    "AND cmd.cbMarq = '" + cbMarq + "'";
            }
            else
            {
                return "SELECT DISTINCT(DO_Piece)as DO_Piece, cli.CT_Num, CONCAT(cli.CT_Complement,',',cli.CT_CodePostal,',',cli.CT_Ville, ',',cli.CT_Pays)as Adresse, cmd.DO_DEVISE, cmd.DO_Date, cmd.DO_DateLivr, cmd.DO_Condition, cmd.DO_TotalHT, cli.CT_Intitule, cmd.DO_Motif, cli.CT_EdiCode, cmd.N_CATCOMPTA, liv.LI_Contact, cli.CT_Telephone, cli.CT_EMail " +
                    "FROM F_COMPTET as cli, F_DOCENTETE as cmd, F_livraison liv " +
                    "WHERE cmd.DO_Tiers = cli.CT_Num AND cmd.DO_Tiers = liv.CT_Num " +
                    "AND cmd.cbMarq = '" + cbMarq + "'";
            }
        }

        public static string getRefCMDClient(bool sqlConnexion, string reference_doc)
        {
            if (sqlConnexion)
            {
                return "SELECT DO_Ref, Nature_OP_P, DO_TotalHT, DO_TotalHTNet, DO_TotalTTC, DO_NetAPayer, DO_MontantRegle FROM " + getPrefix() + "F_DOCENTETE WHERE DO_Piece = '"+ reference_doc + "'";
            }
            else
            {
                return "SELECT DO_Ref, Nature_OP_P, DO_TotalHT, DO_TotalHTNet, DO_TotalTTC, DO_NetAPayer, DO_MontantRegle FROM F_DOCENTETE WHERE DO_Piece = '" + reference_doc + "'";
            }
        }

        public static string insertDesadvDocument_Veolog(bool sqlConnexion, string DO_Type, string reference_doc, string curr_date, string veologDeliveryDateTime, Veolog_DESADV dh, string Nature_OP_P, string DO_TotalHT, string DO_TotalHTNet, string DO_TotalTTC, string DO_NetAPayer, string DO_MontantRegle, string[] reference_client, string DO_Expedit)
        {
            if (sqlConnexion)
            {
                string sql = "INSERT INTO " + getPrefix() + "F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DO_CONDITION, DO_DEVISE, DO_LANGUE, DO_NBFACTURE, DO_TXESCOMPTE, N_CATCOMPTA, CO_NO, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_DATE, DO_DATELIVR, DO_DOMAINE, DO_EXPEDIT, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TYPETRANSAC, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, DO_COORD01, COMMENTAIRES, DO_COURS, Nature_OP_P, DO_TotalHT, DO_TotalHTNet, DO_TotalTTC, DO_NetAPayer, DO_MontantRegle) " +
                                "VALUES (" + reference_client[2] + ", '" + reference_client[3] + "', " + reference_client[4] + ", " + reference_client[5] + ", " + reference_client[6] + ", " + reference_client[7] + ", " + reference_client[8] + ", " + reference_client[9] + ", " + reference_client[10] + ", 1, 0, 0, 0, 1, {d '" + curr_date + "'}, {d '"+ veologDeliveryDateTime + "'}, 0, " + DO_Expedit + ", 1, '" + reference_doc + "', '" + dh.Ref_Commande_Client_Livre + "', 21, 2, " + reference_client[11] + ", '" + reference_client[0] + "', 11, 0, " + DO_Type + ", 1, 0, 0, '', '" + reference_doc + ": document from logistic.', 0.000000, '"+Nature_OP_P+"', "+DO_TotalHT+ ", " + DO_TotalHTNet + ", " + DO_TotalTTC + ", " + DO_NetAPayer + ", " + DO_MontantRegle+")";
                return sql;
            }
            else
            {
                string sql = "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DO_CONDITION, DO_DEVISE, DO_LANGUE, DO_NBFACTURE, DO_TXESCOMPTE, N_CATCOMPTA, CO_NO, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_DATE, DO_DATELIVR, DO_DOMAINE, DO_EXPEDIT, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TYPETRANSAC, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, DO_COORD01, COMMENTAIRES, DO_COURS, Nature_OP_P, DO_TotalHT, DO_TotalHTNet, DO_TotalTTC, DO_NetAPayer, DO_MontantRegle)" +
                                "VALUES (" + reference_client[2] + ", '" + reference_client[3] + "', " + reference_client[4] + ", " + reference_client[5] + ", " + reference_client[6] + ", " + reference_client[7] + ", " + reference_client[8] + ", " + reference_client[9] + ", " + reference_client[10] + ", 1, 0, 0, 0, 1, {d '" + curr_date + "'}, {d '"+ veologDeliveryDateTime + "'}, 0, " + DO_Expedit + ", 1, '" + reference_doc + "', '" + dh.Ref_Commande_Client_Livre + "', 21, 2, " + reference_client[11] + ", '" + reference_client[0] + "', 11, 0, " + DO_Type + ", 1, 0, 0, '', '" + reference_doc + ": document from logistic.', 0.000000, '" + Nature_OP_P + "', " + DO_TotalHT + ", " + DO_TotalHTNet + ", " + DO_TotalTTC + ", " + DO_NetAPayer + ", " + DO_MontantRegle +")";
                return sql;
            }
        }

        public static string insertDesadvDocumentLine_Veolog(bool sqlConnexion, string[,] products, int x)
        {
            if (sqlConnexion)
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT INTO " + getPrefix() + "F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte, DL_PUTTC, DL_TTC, DL_PieceBC, DL_PieceBL, DL_DateBL, DL_TNomencl, DL_TRemPied, DL_TRemExep, DL_QteBC, DL_QteBL, DL_Remise01REM_Valeur, DL_Remise01REM_Type, DL_Remise02REM_Valeur, DL_Remise02REM_Type, DL_Remise03REM_Valeur, DL_Remise03REM_Type, DL_NoRef, DL_TypePL, DL_PUDevise, CA_Num, DL_Frais, AC_RefClient, DL_PiecePL, DL_DatePL, DL_QtePL, DL_NoColis, DL_NoLink, CO_No, DT_No, DL_PieceDE, DL_DateDe, DL_QteDE, DL_NoSousTotal, CA_No,     DL_PUBC, DL_CodeTaxe1, DL_Taxe1, DL_Taxe2, DL_Taxe3, DL_TypeTaux1, DL_TypeTaxe1, DL_TypeTaux2, DL_TypeTaxe2, DL_TypeTaux3, DL_TypeTaxe3) " +
                                "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ", "+ products[x, 27] + ", " + products[x, 28] + ", '" + products[x, 29] + "', '" + products[x, 30] + "', {d '" + products[x, 31] + "'}, " + products[x, 32] + ", " + products[x, 33] + ", " + products[x, 34] + ", " + products[x, 35] + ", " + products[x, 36] + ", " + products[x, 37] + ", " + products[x, 38] + ", " + products[x, 39] + ", " + products[x, 40] + ", " + products[x, 41] + ", " + products[x, 42] + ", " + products[x, 43] + ", " + products[x, 44] + ", " + products[x, 45] + ", '" + products[x, 46] + "', " + products[x, 47] + ", '" + products[x, 48] + "', " + products[x, 49] + ", {d '" + products[x, 50] + "'}, " + products[x, 51] + ", '" + products[x, 52] + "', " + products[x, 53] + ", " + products[x, 54] + ", " + products[x, 55] + ", '" + products[x, 56] + "', {d '" + products[x, 57] + "'}, " + products[x, 58] + ", " + products[x, 59] + ", " + products[x, 60] + ", " + products[x, 61] + ", '" + products[x, 62] + "', " + products[x, 63] + ", " + products[x, 64] + ", " + products[x, 65] + ", " + products[x, 66] + ", " + products[x, 67] + ", " + products[x, 68] + ", " + products[x, 69] + ", " + products[x, 70] + ", " + products[x, 71] + ")";
                return sql;
            }
            else
            {
                // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
                //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
                string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte, DL_PUTTC) " +
                                "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ", " + products[x, 27] + ")";
                return sql;
            }
        }

        public static string deleteEnteteDocument(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                return "DELETE FROM "+getPrefix()+ "F_DOCENTETE WHERE DO_Domaine = '"+reference+"'";
            }
            else
            {
                return "DELETE FROM F_DOCENTETE WHERE DO_Domaine = '" + reference + "'";
            }
        }

        public static string deleteLigneDocument(bool sqlConnexion, string reference)
        {
            if (sqlConnexion)
            {
                return "DELETE FROM " + getPrefix() + "F_DOCLIGNE WHERE DO_Domaine = '" + reference + "'";
            }
            else
            {
                return "DELETE FROM F_DOCLIGNE WHERE DO_Domaine = '" + reference + "'";
            }
        }

        public static string checkVelog_PendingTable(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "IF (EXISTS (SELECT * " +
                                    "FROM INFORMATION_SCHEMA.TABLES " +
                                    "WHERE TABLE_SCHEMA = '" + getPrefix() + "' " + //DataBase.dbo
                                    "AND  TABLE_NAME = 'F_DOCENTETE')) " +
                            "BEGIN " +
                                "SELECT FIELD(0, 0); " +    //Return 1
                            "END " +
                        "ELSE " +
                            "BEGIN " +
                                "SELECT FIELD(0, 1); " +    //Return 0
                            "END";
            }
            else
            {
                return "Dont Support ODBC Request";
            }
        }

        public static string createVelog_PendingTable(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "CREATE TABLE " + getPrefix() + "Velog_Pending{" +
                    "ID INT PRIMARY KEY NOT NULL, "+
                    "CMD_cbMarq INT(255)," +
                    "CMD_REF VARCHAR(255)," +
                    "CMD_PIECE VARCHAR(255)," +
                    "CMD_STATUT VARCHAR(255)" +
                    "}";
            }
            else
            {
                return "CREATE TABLE Velog_Pending{" +
                    "ID INT PRIMARY KEY NOT NULL, " +
                    "CMD_cbMarq VARCHAR(255)," +
                    "CMD_REF VARCHAR(255)," +
                    "CMD_PIECE VARCHAR(255)," +
                    "CMD_STATUT VARCHAR(255)" +
                    "}";
            }
        }

        public static string insertOrderInVeolog_Pending(bool sqlConnexion, string cmd_cbMarq, string cmd_DoRef, string cmd_DoPiece, string cmd_statut)
        {
            if (sqlConnexion)
            {
                return "INSERT INTO " + getPrefix() + "Velog_Pending(CMD_cbMarq, CMD_REF, CMD_PIECE, CMD_STATUT) VALUE(" + cmd_cbMarq + ", '"+ cmd_DoRef + "', '" + cmd_DoPiece + "', '" + cmd_statut + "')";
            }
            else
            {
                return "INSERT INTO Velog_Pending(CMD_cbMarq, CMD_REF, CMD_PIECE, CMD_STATUT) VALUE(" + cmd_cbMarq + ", '" + cmd_DoRef + "', '" + cmd_DoPiece + "', '" + cmd_statut + "')";
            }
        }

        public static string getCommandeFromVeolog_Pending(bool sqlConnexion, string cmd_cbMarq)
        {
            if (sqlConnexion)
            {
                return "SELECT CMD_PIECE FROM " + getPrefix() + "Velog_Pending WHERE cbMarq = '"+cmd_cbMarq+"'";
            }
            else
            {
                return "SELECT CMD_PIECE FROM Velog_Pending WHERE cbMarq = '" + cmd_cbMarq + "'";
            }
        }

        #endregion
    }
}
