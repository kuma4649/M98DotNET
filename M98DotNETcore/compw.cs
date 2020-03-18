﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
    public class compw
    {
        public int nte;
        public int oct;
        public int len;        //カウンタであることに注意
        public int clk;
        public int msl;        //変換中で一番短いレングスのカウンタ
        public int qtz;
        public int vol;
        public int voi;
        public int kst;
        public int pan;

        public compw(int nt, int ot, int ln, int ms, int cl, int qt, int vl, int vi, int ks, int pa)
        {
            nte = nt; oct = ot; vol = vl; qtz = qt; len = ln; clk = cl; voi = vi;
            kst = ks; msl = ms; pan = pa;
        }

        public void initcwk()
        {
            nte = 0; oct = 4; len = 6; clk = 96; msl = 255;
            qtz = DEF.EMPTY; vol = DEF.EMPTY; voi = DEF.EMPTY; kst = DEF.EMPTY; pan = DEF.EMPTY;
        }

    }
}
