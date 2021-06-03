using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Classes
{
    public class OrderLines
    {
        public string cbMarq;
        public string do_piece;
        public string dl_design;
        public string date;
        public string dl_qte;
        public string dl_qtepl;
        public string dl_ligne;
        public string ar_ref;
        public string ar_codebarre;
        public bool isReliquat;

        public OrderLines() { }

        public OrderLines(string cbMarq, string dl_design, string do_piece, string date, string dl_qte, string dl_qtepl, string dl_ligne, string ar_ref, string ar_codebarre)
        {
            this.cbMarq = cbMarq;
            this.dl_design = dl_design;
            this.do_piece = do_piece;
            this.date = date;
            this.dl_qte = dl_qte;
            this.dl_qtepl = dl_qtepl;
            this.dl_ligne = dl_ligne;
            this.ar_ref = ar_ref; 
            this.ar_codebarre = ar_codebarre;
        }
    }
}
