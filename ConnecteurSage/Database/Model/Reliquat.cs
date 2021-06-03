using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class Reliquat
    {
        public string id;
        public string date;
        public string modification;
        public string do_piece;
        public string do_ref;
        public string dl_ligne;
        public string dl_design;
        public string dl_qte;
        public string dl_qtepl;
        public string ar_ref;
        public string ar_codebarre;

        public Reliquat() { }
        public Reliquat(string id, string date, string modification, string do_piece, string do_ref, string dl_ligne, string dl_design, string dl_qte, string dl_qtepl, string ar_ref, string ar_codebarre)
        {
            this.id = id;
            this.date = date;
            this.modification = modification;
            this.do_piece = do_piece;
            this.do_ref = do_ref; 
            this.dl_ligne = dl_ligne; 
            this.dl_design = dl_design; 
            this.dl_qte = dl_qte; 
            this.dl_qtepl = dl_qtepl; 
            this.ar_ref = ar_ref; 
            this.ar_codebarre = ar_codebarre;
        }
    }
}
