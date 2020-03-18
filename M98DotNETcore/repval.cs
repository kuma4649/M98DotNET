namespace M98DotNETcore
{
	public class repval
	{
		public int min, max;   //value continue
		public int c_nte;
		public int c_vol;
		public int c_voi;
		public int c_len;
		public int c_qtz;
		public int c_kst;
		public int c_ext;
		public int c_pan;
		public int c_clk;
		public flag f_nte;
		public flag f_vol;
		public flag f_voi;
		public flag f_len;
		public flag f_qtz;
		public flag f_kst;
		public flag f_ext;
		public flag f_pan;
		public flag f_clk;

		public repval(int a, flag b)
		{
			min = max = c_nte = c_vol = c_voi = c_len = c_qtz = c_kst = c_ext = c_pan = c_clk = a;
			f_nte = f_vol = f_voi = f_len = f_qtz = f_kst = f_ext = f_pan = f_clk = b;
		}

		public void allcof()
		{
			c_nte = c_vol = c_voi = c_len = c_qtz = c_kst = c_ext = c_pan = c_clk = 0;
		}
		public void allfof()
		{
			f_nte = f_vol = f_voi = f_len = f_qtz = f_kst = f_ext = f_pan = f_clk = flag.OFF;
		}
		public void allfon()
		{
			f_nte = f_vol = f_voi = f_len = f_qtz = f_kst = f_ext = f_pan = f_clk = flag.ON;
		}
		public void s_rep()
		{
			if (min > max) { min = max = 0; }
			if (f_nte == flag.ON) { c_nte = gWork.random.Next((max - min) + 1) + min; }
			if (f_vol == flag.ON) { c_vol = gWork.random.Next((max - min) + 1) + min; }
			if (f_len == flag.ON) { c_len = gWork.random.Next((max - min) + 1) + min; }
			if (f_qtz == flag.ON) { c_qtz = gWork.random.Next((max - min) + 1) + min; }
			if (f_voi == flag.ON) { c_voi = gWork.random.Next((max - min) + 1) + min; }
			if (f_kst == flag.ON) { c_kst = gWork.random.Next((max - min) + 1) + min; }
			if (f_ext == flag.ON) { c_ext = gWork.random.Next((max - min) + 1) + min; }
			if (f_pan == flag.ON) { c_pan = gWork.random.Next((max - min) + 1) + min; }
			if (f_clk == flag.ON) { c_clk = gWork.random.Next((max - min) + 1) + min; }
		}
	}
}
