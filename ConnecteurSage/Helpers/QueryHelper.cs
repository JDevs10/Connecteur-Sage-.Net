﻿using System;
using System.Collections.Generic;
using System.Text;
using ConnecteurSage.Classes;

namespace ConnecteurSage.Helpers
{
  /// <summary>
  /// Classe statique permettant de stocker toutes les requêtes SQL utilisées dans l'application
  /// </summary>
  public static class QueryHelper
  {
    #region SQL Queries
 

   public static string getClient(string id)
   {
       return "SELECT CT_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise,  N_Expedition, CT_Langue, CT_Facture, N_Period, N_CatTarif, CT_Taux02, N_CatCompta FROM F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=0";
   }

   public static string fournisseurExiste(string id)
   {
       return "SELECT CT_Num FROM F_COMPTET where CT_EdiCode='" + id + "' and CT_Type=1";
   }

   public static string getArticle(string id)
   {
       return "SELECT AR_REF,AR_SuiviStock,AR_Gamme1,AR_Gamme2,AR_Nomencl,RP_CODEDEFAUT,AR_PRIXVEN,AR_POIDSBRUT,AR_POIDSNET,AR_UnitePoids,AR_DESIGN from F_ARTICLE where AR_CODEBARRE='" + id + "'";
   }

   public static string getConditionnementArticle(string refArt)
   {
       return "select EC_ENUMERE,EC_QUANTITE FROM F_CONDITION where CO_Principal = 1 and AR_REF='" + refArt + "'";
   } 

   public static string getStockId()
   {
       return "select DE_NO FROM F_Depot where DE_Principal = 1";
   }

   public static string getNumLivraison(string ct_num)
   {
       return "select LI_NO FROM F_LIVRAISON where CT_NUM = '" + ct_num + "' and li_principal=1";
   }

   public static string get_last_Num_Livraison(string client)
   {
       return "select max(LI_NO) FROM F_LIVRAISON WHERE CT_NUM='"+client+"'";
   }

   public static string deleteCommande(string numCommande)
   {
       return "delete from F_docentete where DO_COORD01='" + numCommande + "'";
   }

   public static string UpdateCommandeTaxes(string montantTaxes, string do_piece)
   {
       return "UPDATE F_DOCENTETE SET FNT_MONTANTTOTALTAXES=" + montantTaxes + " WHERE DO_PIECE='" + do_piece + "'";
   }

   public static string getGAMME(int type,string REF_Article)
   {
       return "select AG_NO from F_artgamme where AG_TYPE="+type+" and AR_REF='" + REF_Article + "'";
   }



   public static string insertCommande(Client client, Order order)
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

   public static string insertLigneCommande(Client client, Order order, OrderLine line)
   {
       return "Insert Into " +
            "F_DOCLIGNE(AC_REFCLIENT,AF_REFFOURNISS,AR_REF,CT_NUM,DE_NO," +
            "DL_DATEBC,DL_DESIGN,DL_LIGNE,DL_PRIXUNITAIRE,DL_QTE,EU_Enumere,EU_QTE," +
            "DL_VALORISE,DO_DATE,DO_DATELIVR,DO_DOMAINE," +
            "DO_PIECE,DO_TYPE,AG_No1,AG_No2,AR_RefCompose,RP_Code,do_ref,DL_PoidsNet,DL_PoidsBrut,DL_Taxe1,DL_TypeTaux1,DL_TypeTaxe1) " +
            "values " +
            "('" + line.codeAcheteur + "','" + line.codeFournis + "','" + line.article.AR_REF +"','" + client.CT_Num + "'," + line.article.AR_StockId + ", " +
            "'{d " + order.DateCommande + "}','" + line.descriptionArticle + "'," + line.NumLigne + "," + line.PrixNetHT + "," + line.Quantite + ",'" + (line.article.Conditionnement != null ? line.article.Conditionnement.EC_ENUMERE : "") + "'," + (line.Calcule_conditionnement != "0" ? line.Calcule_conditionnement : line.Quantite) + "," +
            "1,'{d " + order.DateCommande + "}'," + line.DateLivraison + ",0,'" + order.Id + "',1," + line.article.gamme1 + "," + line.article.gamme2 + ",'" + line.article.AR_REFCompose + "','" + line.article.RP_CODEDEFAUT + "','" + line.codeFournis + "'," + line.article.AR_POIDSNET.Replace(",", ".") + "," + line.article.AR_POIDSBRUT.Replace(",", ".") + ",20,0,0)";
   }

   public static string getInfoSociete()
   {
       return "SELECT D_RaisonSoc,D_Adresse,D_CodePostal,D_Ville,D_Pays,D_Siret,D_Identifiant,D_Commentaire from p_dossier";
   }

   public static string getGNLClientLivraison(string intitule)
   {
       return "SELECT CT_EdiCode from f_comptet where CT_intitule='"+ intitule +"'";
   }

   public static string MaxNumPiece()
   {
       return "SELECT Max(F_DOCENTETE.DO_PIECE) FROM F_DOCENTETE F_DOCENTETE WHERE (F_DOCENTETE.DO_DOMAINE=0) AND (F_DOCENTETE.DO_TYPE=1)";
   }

   public static string get_NumPiece_Motif(string num)
   {
       return "SELECT DO_PIECE FROM F_DOCENTETE WHERE DO_COORD01='" + num + "'";
   }

   public static string get_Next_NumPiece_BonCommande()
   {
       return "SELECT DC_PIECE FROM F_DOCCURRENTPIECE WHERE DC_IDCOL=1 and DC_SOUCHE=0";
   }

   public static string get_condition_livraison_indice(string c_mode)
   {
       return "SELECT p_Condlivr.CBINDICE FROM p_Condlivr p_Condlivr WHERE (p_Condlivr.C_MODE Like '" + c_mode + "%')";
   }

   public static string get_condition_livraison_mode(string indice)
   {
       return "SELECT C_MODE FROM p_Condlivr WHERE CBINDICE=" + indice + "";
   }

   public static string get_adresse_livraison(AdresseLivraison adresse)
   {
       return "Select li_no,li_contact,Li_adresse,li_codepostal,li_ville,li_pays from f_livraison where li_pays='" + adresse.pays + "' and li_codepostal='" + adresse.codePostale + "' and li_ville='" + adresse.ville + "' and Li_adresse='" + adresse.adresse + "' and li_contact='" + adresse.Nom_contact + "' and CT_NUM='"+adresse.CT_NUM+"'";
   }

   public static string insert_adresse_livraison(string client, AdresseLivraison adresse)
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

   public static string TestSiNumPieceExisteDeja(string num)
   {
       return "select do_piece from f_docentete where do_piece='" + num + "'";
   }

   public static string TestIntituleLivraison(string LI_intitule)
   {
       return "select LI_intitule FROM F_LIVRAISON where LI_intitule like '" + LI_intitule + "%'";
   }
      
// ******************************************************************************************

   public static string getDevise(string codeIso)
   {
       return "select CBINDICE from P_DEVISE where D_CODEISO='" + codeIso + "'";
   }

   public static string getDeviseIso(string code)
   {
       return "select D_CODEISO from P_DEVISE where CBINDICE=" + code + "";
   }

   public static string getListCommandes()
   {
       return "SELECT doc.DO_PIECE, cli.CT_EdiCode, liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_COMPLEMENT, liv.LI_VILLE, liv.LI_PAYS, doc.DO_DEVISE, doc.DO_DATE, doc.DO_DATELIVR, cond.C_MODE, doc.FNT_TOTALHTNET,doc.do_tiers,doc.do_motif,doc.do_coord01,liv.li_contact "+
"FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv "+
"WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=1) AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition) AND (cli.CT_NUM=doc.do_tiers)";
   }

   public static string getListLignesCommandes(string codeCommande)
   {
       return "SELECT doc.DL_LIGNE, art.AR_CODEBARRE, doc.DL_DESIGN, doc.DL_QTE, doc.DL_PRIXUNITAIRE, doc.DL_MONTANTHT, doc.DO_DATELIVR, doc.do_ref, doc.AC_REFCLIENT "+
"FROM F_ARTICLE art, F_DOCLIGNE doc "+
"WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeCommande + "'";
   }

      
// ******************************************************************************************

   public static string getListDocumentVente(string client,int type)
   {
       if(type == 67)
       {
            return "SELECT doc.DO_Piece,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num " +
"FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv " +
"WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=5 or doc.DO_TYPE=6 or doc.DO_TYPE=7) AND (doc.DO_TIERS='" + client + "') AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers)";
       }
       // J'ai modifier DO_COORD01 par DO_REF pour le client TRACE SPORT
       return "SELECT doc.DO_Piece,doc.DO_date,doc.DO_dateLivr,doc.DO_devise,doc.LI_No,doc.DO_Statut,doc.DO_taxe1,doc.DO_taxe2,doc.DO_taxe3,doc.DO_TypeTaxe1,doc.DO_TypeTaxe2,doc.DO_TypeTaxe3,doc.FNT_MontantEcheance,doc.FNT_MontantTotalTaxes,doc.FNT_NetAPayer,doc.FNT_PoidsBrut,doc.FNT_PoidsNet,doc.FNT_Escompte,doc.FNT_TotalHT,doc.FNT_TotalHTNet,doc.FNT_TotalTTC,liv.LI_ADRESSE, liv.LI_CODEPOSTAL, liv.LI_CODEREGION, liv.LI_EMAIL, liv.LI_VILLE, liv.LI_PAYS, cond.C_MODE,doc.DO_REF, liv.LI_INTITULE,doc.do_motif,do_txescompte,doc.ca_num " +
"FROM F_comptet cli, P_condlivr cond, F_docentete doc, F_LIVRAISON liv " +
"WHERE (doc.DO_DOMAINE=0) AND (doc.DO_TYPE=" + type + ") AND (doc.DO_TIERS='" + client + "') AND (doc.LI_NO=liv.LI_NO) AND (cond.CBINDICE=doc.do_condition)  AND (cli.CT_NUM=doc.do_tiers)";
   }

   public static string getListDocumentVenteLine(string codeDocument)
   {
       return "SELECT doc.DO_Date,doc.DO_DateLivr,doc.DL_Ligne,doc.AR_Ref,doc.DL_Design,doc.DL_Qte,doc.DL_QteBC,doc.DL_QteBL,doc.EU_Qte,doc.DL_PoidsNet,doc.DL_PoidsBrut,doc.DL_Remise01REM_Valeur,doc.DL_Remise01REM_Type,doc.DL_Remise03REM_Valeur,doc.DL_Remise03REM_Type,doc.DL_PrixUnitaire,doc.DL_Taxe1,doc.DL_Taxe2,doc.DL_Taxe3,doc.DL_TypeTaxe1,doc.DL_TypeTaxe2,doc.DL_TypeTaxe3,doc.DL_MontantHT,doc.DL_MontantTTC,doc.DL_NoColis,doc.FNT_MontantHT,doc.FNT_MontantTaxes,doc.FNT_MontantTTC,doc.FNT_PrixUNet,doc.FNT_PrixUNetTTC,doc.FNT_RemiseGlobale,art.AR_CODEBARRE " +
"FROM F_ARTICLE art, F_DOCLIGNE doc " +
"WHERE doc.AR_REF = art.AR_REF and doc.do_piece='" + codeDocument + "'";
   }

   public static string getListClient()
   {
       return "SELECT CT_Num,CT_Intitule,CT_Adresse,CT_APE,CT_CodePostal,CT_CodeRegion,CT_Complement,CT_CONTACT,CT_EdiCode,CT_email,CT_Identifiant, CT_Ville,CT_Pays,CT_Siret,CT_Telephone,N_DEVISE,CT_SvFormeJuri  FROM F_COMPTET where CT_Type=0";
   }

   public static string getModeReglement(string do_piece)
   {
       return "SELECT INT_REGLEMENT,DR_DATE,DR_TYPEREGL,DR_POURCENT,DR_MONTANT from f_docregl where do_piece='" + do_piece + "'";
   }

    /* GET STOCK INFORMATION WITH PRODUCTS */
   public static string getStockInfo()
   {
       return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM F_LOTSERIE as ArtLot, F_ARTICLE as Art, F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
       // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
   }

    /* INSERT STOCK MOVEMENT */
   public static string insertStockInfo(Stock s)
   {
       
       return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM F_LOTSERIE as ArtLot, F_ARTICLE as Art, F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
       // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
   }

   public static string insertDesadv(Desadv d)
   {

       return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM F_LOTSERIE as ArtLot, F_ARTICLE as Art, F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
       // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
   }

   public static string insertDesadvLine(DesadvLine dl)
   {

       return "SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM F_LOTSERIE as ArtLot, F_ARTICLE as Art, F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC";
       // testing : SELECT Art.AR_Design as libelle,Art.AR_Ref as ref, Art.AR_CodeBarre as barcode, ArtStock.AS_QteSto as stock,ArtLot.LS_NoSerie,ArtLot.LS_Qte,ArtLot.LS_LotEpuise FROM BIJOU.dbo.F_LOTSERIE as ArtLot, BIJOU.dbo.F_ARTICLE as Art, BIJOU.dbo.F_ARTSTOCK as ArtStock WHERE Art.AR_Ref=ArtStock.AR_Ref AND Art.AR_Ref= ArtLot.AR_Ref ORDER BY ArtStock.AS_QteSto DESC
   }

   public static string getLastPieceNumberReference(string mask)
   {
       return "SELECT DO_Piece FROM F_DOCENTETE WHERE DO_Piece LIKE '" + mask + "%' ORDER BY DO_Piece DESC";
   }

   public static string getNegativeStockOfAProduct(string reference)
   {
       //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
       return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'MS%' OR DO_Piece LIKE 'MT%' OR DO_Piece LIKE 'CA%' OR DO_Piece LIKE 'BL%' OR DO_Piece LIKE 'FA%') AND AR_Ref IN('" + reference + "') ";
   }

   public static string getPositiveStockOfAProduct(string reference) 
   {
       //return "SELECT DISTINCT(DO_Piece),DO_Ref,AR_Ref,DL_Qte,DL_Design FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
       return "SELECT DO_Piece,DO_Ref,AR_Ref,DL_Qte,DL_Design,DL_Ligne FROM F_DOCLIGNE WHERE (DO_Piece LIKE 'FFA%' OR DO_Piece LIKE 'ME%' OR DO_Piece LIKE 'FBL%' ) AND AR_Ref IN('" + reference + "') ";
   }

   public static string getProductNameByReference(string reference)
   {
       return "SELECT AR_Design,AR_PoidsNet,AR_PoidsBrut,AR_PrixAch FROM F_ARTICLE WHERE AR_Ref IN('" + reference + "') ";
   }

   public static string insertStockDocument(string DO_Type, string reference_doc, string curr_date, string curr_date_seconds, string curr_date_time)
   {
      /* string sql = "";

       sql = sql + "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, Commentaires)";
       sql = sql + "VALUES ('', '', 0, 0, 0, 0, 1, 0, {d '" + curr_date + "'}, NULL, 0, 2, 0   , 0, 0 , 0, '" + reference_doc + "', '" + curr_date_seconds + "', 0, 0 , 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '','" + reference_doc + ":document from logistic.')";

       return sql;/*
       //return "INSERT INTO F_DOCENTETE ( DO_Domaine, DO_Type, DO_Piece, DO_Date, DO_Ref, DO_Tiers, Commentaires ) VALUES (2,"+DO_Type+",'"+reference_doc+"','"+curr_date+"', CONCAT('ME00004:IMPORT le ',NOW()), '1','ME00004:document from logistic.')";
       //new return "INSERT INTO F_DOCENTETE ( DO_Domaine, DO_Type, DO_Piece, DO_Date, DO_Ref, DO_Tiers, Commentaires ) VALUES (2," + DO_Type + ",'" + reference_doc + "', {d '" + curr_date + "'}, '" + curr_date_seconds + "', '1','" + reference_doc + ":document from logistic.')";
      
       /*
       return "INSERT INTO F_DOCENTETE (DO_Domaine, DO_Type, DO_Piece, DO_Date, DO_Ref, DO_Tiers, DO_Colisage, DO_TypeColis, DO_TotalHT, Commentaires) VALUES ("+
              "2, " + DO_Type + ", '" + reference_doc + "', {d '" + curr_date + "'}, '" + curr_date_seconds + "', '1', 1, 1, " + total_ht + ", '" + reference_doc + ": document from logistic a " + curr_date_time + ".')";
       */
       
        /*
       return "INSERT INTO F_DOCENTETE (DO_Domaine, DO_Type, DO_DocType, DO_Piece, DO_Date, DO_Ref, DO_CLOTURE, DO_COLISAGE, DO_Tiers, CO_No, DO_Period, DO_Devise, DO_Cours, DE_No, LI_No, DO_Expedit, DO_NbFacture, DO_BLFact, DO_TxEscompte, "+
           " DO_Reliquat, DO_Imprim, CA_Num, DO_COORD01, DO_COORD02, DO_COORD03, DO_COORD04, DO_Souche, DO_DATELIVR, DO_CONDITION, DO_Tarif, DO_Transaction, DO_Langue, DO_Ecart, DO_Regime, N_CatCompta, DO_Ventile, AB_No, DO_DebutAbo, "+
           "DO_FinAbo, DO_DebutPeriod, DO_FinPeriod, CG_Num, DO_Statut, DO_Heure, CA_No, CO_NoCaissier, DO_Transfere, DO_NoWeb, DO_Attente, DO_Provenance, CA_NumIFRS, MR_No, DO_TypeFrais, DO_TypeFranco, DO_ValFranco, DO_TypeLigneFranco, "+
           "DO_Motif, DO_Contact, DO_FactureElec, DO_TypeTransac, DO_FactureFrs, DO_PieceOrig, DO_EStatut, DO_DemandeRegul, ET_No, DO_Valide, DO_Coffre, DO_TotalHT, DO_StatutBAP, DO_Escompte, Commentaires) VALUES(" +
           "2, " + DO_Type + ", "+DO_Type+", '" + reference_doc + "', '" + curr_date + "', '" + curr_date_seconds + "', 1, 1, '1', 0, 0, 0, 0.000000, 0, 0, 0, 0, 0, 0, 0, 0, '', '', '', '', '', 0, NULL, NULL, 0, 0, 0, 0.000000, 0, 0, 0, 0, "+
           " NULL, NULL, NULL, NULL, NULL, 0, 0.000000, 0, 0, 0, 0, 0, 0, '', 0, 0.000000, 0, 0.000000, 0, '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, " + total_ht + ", 0, 0, '"+reference_doc+": document from logistic à "+date_time+"')";
       */
       /*
       return "INSERT INTO F_DOCENTETE (DO_Domaine, DO_Type, DO_DocType, DO_Piece, DO_Date, DO_Ref, DO_CLOTURE, DO_COLISAGE, DO_Tiers, CO_No, DO_Period, DO_Devise, DO_Cours, DE_No, LI_No, DO_Expedit, DO_NbFacture, DO_BLFact, DO_TxEscompte, " +
           " DO_Reliquat, DO_Imprim, CA_Num, DO_COORD01, DO_COORD02, DO_COORD03, DO_COORD04, DO_Souche, DO_Tarif, DO_Transaction, DO_Langue, DO_Ecart, DO_Regime, N_CatCompta, DO_Ventile, AB_No, " +
           " DO_Statut, DO_Heure, CA_No, CO_NoCaissier, DO_Transfere, DO_NoWeb, DO_Attente, DO_Provenance, CA_NumIFRS, MR_No, DO_TypeFrais, DO_TypeFranco, DO_ValFranco, DO_TypeLigneFranco, " +
           "DO_Motif, DO_Contact, DO_FactureElec, DO_TypeTransac, DO_FactureFrs, DO_PieceOrig, DO_EStatut, DO_DemandeRegul, ET_No, DO_Valide, DO_Coffre, DO_TotalHT, DO_StatutBAP, DO_Escompte, Commentaires) VALUES(" +
           "2, " + DO_Type + ", " + DO_Type + ", '" + reference_doc + "', {d '" + curr_date + "'}, '" + curr_date_seconds + "', 1, 1, '1', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '', '', '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, " +
           "0, 0, 0, 0, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, " + total_ht + ", 0, 0, '" + reference_doc + ": document from logistic à " + curr_date_time + "')";
        */
       /*
       string sql = "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, Commentaires)"+
           "VALUES ('37100', 'EDF', 0, 0, 0, 0, 1, 0,'20190920', NULL, 0, 2, 0 , 0, 0 , 0, 'MS00017', '2019921175800', 0, 0 , 0, 1, 0, 0, 21, 1, 0, 0, 0, '','MS00017:document from logistic.')";
       */
       string sql = "INSERT INTO F_DOCENTETE (CG_NUM, CT_NUMPAYEUR, DE_NO, DO_ATTENTE, DO_BLFACT, DO_CLOTURE, DO_COLISAGE, DO_CONDITION, DO_DATE, DO_DATELIVR, DO_DEVISE, DO_DOMAINE, DO_EXPEDIT, DO_LANGUE, DO_NBFACTURE, DO_PERIOD, DO_PIECE, DO_REF, DO_REGIME, DO_STATUT, DO_TARIF, DO_TIERS, DO_TRANSACTION, DO_TXESCOMPTE, DO_TYPE, DO_TYPECOLIS, DO_VENTILE, LI_NO, N_CATCOMPTA, DO_COORD01, Commentaires)"+
           "VALUES (NULL, NULL, 0, 0, 0, 0, 1, 0,{d '" + curr_date + "'}, NULL, 0, 2, 0 , 0, 0 , 0, '" + reference_doc + "', '" + curr_date_seconds + "', 0, 0 , 0, 1, 0, 0, " + DO_Type + ", 1, 0, 0, 0, '','" + reference_doc + ":document from logistic.')";

       return sql;
   }

   public static string getLatestDL_No()
      {
          return "SELECT DL_No FROM F_DOCLIGNE ORDER BY DL_No DESC";
      }

   public static string insertStockDocumentLine(string[,] products, int x)
   {
       /*
       return "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, DO_Piece, DO_Date, DL_DateBL, DL_Ligne, DO_Ref, AR_Ref, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, " +
            "EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num ) VALUES ( " + products[x, 0] + ", " + products[x, 1] + ", "+products[x,2]+", '" + products[x, 3] + "', '" + products[x, 4] + "', '" + products[x, 5] + "'," +
            " " + products[x, 6] + ", '" + products[x, 7] + "', '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", " + products[x, 12] + "," +
            " " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", " + products[x, 19] + ", '" + products[x, 20] + "')";
       */
       /*
       return "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_Ligne, DO_Ref, AR_Ref, DL_Design, DL_Qte, DL_PoidsNet, "+
           "DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num) VALUES ("+
           ""+products[x,0]+", "+products[x,1]+", "+products[x,2]+", '"+products[x,3]+"', '"+products[x,4]+"', {d '"+products[x,5]+"'}, "+products[x,6]+", '"+products[x,7]+"', "+
           "'"+products[x,8]+"', '"+products[x,12]+"', "+products[x,13]+", "+products[x,14]+", "+products[x,15]+", "+products[x,16]+", "+products[x,17]+", "+products[x,18]+", "+
           ""+products[x,19]+", "+products[x,20]+", "+products[x,21]+", "+products[x,22]+", '"+products[x,23]+"')";
       */
       /*
       return "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_Ligne, DO_Ref, AR_Ref, DL_Design, DL_Qte, DL_PoidsNet, " +
           "DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No) VALUES (" +
           "" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, " + products[x, 6] + ", '" + products[x, 7] + "', " +
           "'" + products[x, 8] + "', '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", " +
           "" + products[x, 19] + ", " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', "+products[x, 24]+")";
       */
       /*
       return "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, " +
           "DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', " +
           "{d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " +
           "" + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", "+
           "" + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ")";
       */
      
       // INSERT INTO BIJOU.dbo.F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00016', {d '2019-09-19'}, {d '2019-09-19'}, 0, '201991917544', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)
       //string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) VALUES (2, 21, 21, '1', 'MS00017', {d '2019-09-23'}, {d '2019-09-23'}, 0, '2019921175800', 'BAAR01', 1, 1, 'Bague Argent', 28, 118.44, 420.000000, 186.000000, 186.000000, 186, '186', 64.000000, 5208.0, 5208.000000, '', 0, 0, 0)";
       string sql = "INSERT INTO F_DOCLIGNE (DO_Domaine, DO_Type, DO_DocType, CT_Num, DO_Piece, DO_Date, DL_DateBC, DL_Ligne, DO_Ref, AR_Ref, DL_Valorise, DE_No, DL_Design, DL_Qte, DL_PoidsNet, DL_PoidsBrut, DL_PrixUnitaire, DL_PrixRU, DL_CMUP, EU_Enumere, EU_Qte, DL_MontantHT, DL_MontantTTC, PF_Num, DL_No, DL_FactPoids, DL_Escompte) "+
           "VALUES (" + products[x, 0] + ", " + products[x, 1] + ", " + products[x, 2] + ", '" + products[x, 3] + "', '" + products[x, 4] + "', {d '" + products[x, 5] + "'}, {d '" + products[x, 6] + "'}, " + products[x, 7] + ", '" + products[x, 8] + "', '" + products[x, 9] + "', " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', " + products[x, 13] + ", " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", " + products[x, 17] + ", " + products[x, 18] + ", '" + products[x, 19] + "', " + products[x, 20] + ", " + products[x, 21] + ", " + products[x, 22] + ", '" + products[x, 23] + "', " + products[x, 24] + ", " + products[x, 25] + ", " + products[x, 26] + ")";
       
       /*sql = sql + "INSERT INTO F_DOCLIGNE(AC_REFCLIENT, AF_REFFOURNISS, CT_NUM, DE_NO, DL_VALORISE, DO_DATELIVR, AG_No1, AG_No2, AR_RefCompose, DL_Taxe1, DL_TypeTaux1, DL_TypeTaxe1, do_ref, AR_REF, DL_PoidsNet, DL_PoidsBrut, DO_DOMAINE, DO_PIECE, DO_TYPE, DO_DATE, DL_DATEBC, DL_DESIGN, DL_LIGNE, DL_PRIXUNITAIRE, DL_QTE, EU_Enumere, EU_QTE) " +
           "VALUES ('" + products[x, 0] + "', '" + products[x, 1] + "', '" + products[x, 2] + "', " + products[x, 3] + ", " + products[x, 4] + ", " + products[x, 5] + ", " + products[x, 6] + ", " + products[x, 7] + ", '" + products[x, 8] + "', " + products[x, 9] + ", " + products[x, 10] + ", " + products[x, 11] + ", '" + products[x, 12] + "', "+
           "'" + products[x, 13] + "', " + products[x, 14] + ", " + products[x, 15] + ", " + products[x, 16] + ", '" + products[x, 17] + "', " + products[x, 18] + ", {d '" + products[x, 19] + "'}, {d '" + products[x, 20] + "'}, '" + products[x, 21] + "', " + products[x, 22] + ", " + products[x, 23] + ", " + products[x, 24] + ", '" + products[x, 25] + "', " + products[x, 26] + ")";
       */return sql;
   }

    #endregion

  }
}
