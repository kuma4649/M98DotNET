using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M98DotNETcore
{
    public class attrib : flgdat
    {
        public atrb[] nte;
        public atrb[] vol;
        public atrb[] voi;
        public atrb[] len;
        public atrb[] qtz;
        public atrb[] ptm;
        public atrb[] mac;
        public atrb[] kst;
        public atrb[] pan;
        public atrb[] clk;
        public atrb[] ext;

        public attrib(flag a) : base(a)
        {
            nte = new atrb[DEF.MAXTONE + 1];
            vol = new atrb[DEF.MAXTONE + 1];
            voi = new atrb[DEF.MAXTONE + 1];
            len = new atrb[DEF.MAXTONE + 1];
            qtz = new atrb[DEF.MAXTONE + 1];
            kst = new atrb[DEF.MAXTONE + 1];
            pan = new atrb[DEF.MAXTONE + 1];
            ptm = new atrb[DEF.MAXTONE + 1];
            mac = new atrb[DEF.MAXTONE + 1];
            clk = new atrb[DEF.MAXTONE + 1];
            ext = new atrb[DEF.MAXTONE + 1];

            reset();
        }

        ~attrib()
        {
            nte = null;
            vol = null;
            voi = null;
            len = null;
            qtz = null;
            kst = null;
            pan = null;
            ptm = null;
            mac = null;
            clk = null;
            ext = null;
        }

        public void disable_flag() //各変換フラグの禁止
        {
            int i = 0;
            if (fnte == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { nte[i] = atrb.DISABLE; }
            if (fvol == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { vol[i] = atrb.DISABLE; }
            if (flen == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { len[i] = atrb.DISABLE; }
            if (fqtz == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { qtz[i] = atrb.DISABLE; }
            if (fvoi == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { voi[i] = atrb.DISABLE; }
            if (fmac == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { mac[i] = atrb.DISABLE; }
            if (fptm == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { ptm[i] = atrb.DISABLE; }
            if (fpan == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { pan[i] = atrb.DISABLE; }
            if (fkst == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { kst[i] = atrb.DISABLE; }
            if (fclk == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { clk[i] = atrb.DISABLE; }
            if (fext == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { ext[i] = atrb.DISABLE; }
        }

        public void enable_flag()  //各変換フラグの許可
        {
            int i = 0;
            if (fnte == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { nte[i] = atrb.INIT; }
            if (fvol == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { vol[i] = atrb.INIT; }
            if (flen == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { len[i] = atrb.INIT; }
            if (fqtz == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { qtz[i] = atrb.INIT; }
            if (fvoi == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { voi[i] = atrb.INIT; }
            if (fmac == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { mac[i] = atrb.INIT; }
            if (fptm == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { ptm[i] = atrb.INIT; }
            if (fpan == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { pan[i] = atrb.INIT; }
            if (fkst == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { kst[i] = atrb.INIT; }
            if (fclk == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { clk[i] = atrb.INIT; }
            if (fext == flag.ON) for (i = 0; i <= DEF.MAXTONE; i++) { ext[i] = atrb.INIT; }
        }

        public void reset()    //すべての属性を無条件クリア
        {
            for (int i = 0; i <= DEF.MAXTONE; i++)
            {
                nte[i] = vol[i] = voi[i] = len[i] = qtz[i] =
                kst[i] = pan[i] = ptm[i] = mac[i] = clk[i] =
                ext[i] = atrb.INIT;
            }
        }

        public void putnte(int point, atrb data) { if (fnte == flag.ON && nte[point] != atrb.MASK && nte[point] != atrb.DISABLE) nte[point] = data; }
        public void putvol(int point, atrb data) { if (fvol == flag.ON && vol[point] != atrb.MASK && vol[point] != atrb.DISABLE) vol[point] = data; }
        public void putvoi(int point, atrb data) { if (fvoi == flag.ON && voi[point] != atrb.MASK && voi[point] != atrb.DISABLE) voi[point] = data; }
        public void putlen(int point, atrb data) { if (flen == flag.ON && len[point] != atrb.MASK && len[point] != atrb.DISABLE) len[point] = data; }
        public void putqtz(int point, atrb data) { if (fqtz == flag.ON && qtz[point] != atrb.MASK && qtz[point] != atrb.DISABLE) qtz[point] = data; }
        public void putptm(int point, atrb data) { if (fptm == flag.ON && ptm[point] != atrb.MASK && ptm[point] != atrb.DISABLE) ptm[point] = data; }
        public void putmac(int point, atrb data) { if (fmac == flag.ON && mac[point] != atrb.MASK && mac[point] != atrb.DISABLE) mac[point] = data; }
        public void putkst(int point, atrb data) { if (fkst == flag.ON && kst[point] != atrb.MASK && kst[point] != atrb.DISABLE) kst[point] = data; }
        public void putpan(int point, atrb data) { if (fpan == flag.ON && pan[point] != atrb.MASK && pan[point] != atrb.DISABLE) pan[point] = data; }
        public void putclk(int point, atrb data) { if (fclk == flag.ON && clk[point] != atrb.MASK && clk[point] != atrb.DISABLE) clk[point] = data; }
        public void putext(int point, atrb data) { if (fext == flag.ON && ext[point] != atrb.MASK && ext[point] != atrb.DISABLE) ext[point] = data; }

        public atrb getnte(int a) { return nte[a]; }
        public atrb getvol(int a) { return vol[a]; }
        public atrb getvoi(int a) { return voi[a]; }
        public atrb getlen(int a) { return len[a]; }
        public atrb getqtz(int a) { return qtz[a]; }
        public atrb getkst(int a) { return kst[a]; }
        public atrb getpan(int a) { return pan[a]; }
        public atrb getptm(int a) { return ptm[a]; }
        public atrb getmac(int a) { return mac[a]; }
        public atrb getclk(int a) { return clk[a]; }
        public atrb getext(int a) { return ext[a]; }

        public void putall(int point, atrb data)
        {
            putnte(point, data);
            putvol(point, data);
            putvoi(point, data);
            putlen(point, data);
            putqtz(point, data);
            putptm(point, data);
            putmac(point, data);
            putkst(point, data);
            putpan(point, data);
            putclk(point, data);
            putext(point, data);
        }

        public void allclr()   //DISABLE属性を除く
                               //すべての属性をクリア
        {
            for (int i = 0; i <= DEF.MAXTONE; i++)
            {
                if (nte[i] != atrb.DISABLE) nte[i] = atrb.INIT;
                if (vol[i] != atrb.DISABLE) vol[i] = atrb.INIT;
                if (voi[i] != atrb.DISABLE) voi[i] = atrb.INIT;
                if (len[i] != atrb.DISABLE) len[i] = atrb.INIT;
                if (qtz[i] != atrb.DISABLE) qtz[i] = atrb.INIT;
                if (ptm[i] != atrb.DISABLE) ptm[i] = atrb.INIT;
                if (mac[i] != atrb.DISABLE) mac[i] = atrb.INIT;
                if (kst[i] != atrb.DISABLE) kst[i] = atrb.INIT;
                if (pan[i] != atrb.DISABLE) pan[i] = atrb.INIT;
                if (clk[i] != atrb.DISABLE) clk[i] = atrb.INIT;
                if (ext[i] != atrb.DISABLE) ext[i] = atrb.INIT;
            }
        }

        public void clr()  //MASK属性とDISABLE属性を除く
                           //すべての属性をクリア
        {
            for (int i = 0; i <= DEF.MAXTONE; i++)
            {
                if (nte[i] != atrb.MASK && nte[i] != atrb.DISABLE) nte[i] = atrb.INIT;
                if (vol[i] != atrb.MASK && vol[i] != atrb.DISABLE) vol[i] = atrb.INIT;
                if (voi[i] != atrb.MASK && voi[i] != atrb.DISABLE) voi[i] = atrb.INIT;
                if (len[i] != atrb.MASK && len[i] != atrb.DISABLE) len[i] = atrb.INIT;
                if (qtz[i] != atrb.MASK && qtz[i] != atrb.DISABLE) qtz[i] = atrb.INIT;
                if (ptm[i] != atrb.MASK && ptm[i] != atrb.DISABLE) ptm[i] = atrb.INIT;
                if (mac[i] != atrb.MASK && mac[i] != atrb.DISABLE) mac[i] = atrb.INIT;
                if (kst[i] != atrb.MASK && kst[i] != atrb.DISABLE) kst[i] = atrb.INIT;
                if (pan[i] != atrb.MASK && pan[i] != atrb.DISABLE) pan[i] = atrb.INIT;
                if (clk[i] != atrb.MASK && clk[i] != atrb.DISABLE) clk[i] = atrb.INIT;
                if (ext[i] != atrb.MASK && ext[i] != atrb.DISABLE) ext[i] = atrb.INIT;
            }
        }

        public new int allflgck()
        {
            return base.allflgck();
        }

    }
}
