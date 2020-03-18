using M98DotNETcore;
using System;
using System.IO;
using System.Text;
using musicDriverInterface;

namespace M98DotNETconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int? seed = null;

            Log.writeLine = write;

            try
            {
                title();//タイトル表示

                //オプションチェック
                int fnIndex = AnalyzeOption(args, ref seed);

                //ファイル名が指定されていない場合はusageを表示して終了
                if (args.Length < fnIndex + 1)
                {
                    DEF.PF("usage: m98.exe [-SEED=n] file1 [file2]");
                    throw new M98Exception();
                }

                string srcFile = args[fnIndex + 0];//変換対象ファイル
                string orgfile = Path.ChangeExtension(args[fnIndex + 0], "ORG");//初版ファイル
                string bupfile = Path.ChangeExtension(args[fnIndex + 0], "BUP");//処理前のファイル
                string saveFn = args[fnIndex + 0];//処理後のファイル。(ソースと同名で上書き保存)
                //引数で別名が指定されていたらそれを使用して保存
                if (args.Length != fnIndex + 1) saveFn = args[fnIndex + 1];

                if (!File.Exists(orgfile))
                {
                    DEF.PF("新規テキストなので、.orgファイルを作成します");
                    File.Copy(srcFile, orgfile);//.orgファイルの作成
                }

                File.Copy(srcFile, bupfile, true);//.bupファイルの作成


                //M98で処理実施
                M98 m98 = new M98(null);
                m98.seed = seed;

                using (FileStream sourceMML = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream destMML = new FileStream(saveFn, FileMode.Create, FileAccess.Write))
                {
                    m98.Preprocess(sourceMML, destMML, null);
                }


            }
            catch (M98Exception)
            {
                Environment.Exit(-1);
            }

            DEF.PF("Complete.");
            Environment.Exit(0);
        }

        private static void write(LogLevel arg1, string arg2)
        {
            Console.WriteLine("[{0}]{1}", arg1, arg2);
        }


        private static void title()
        {
            DEF.PF("M98 ver1.02");
            DEF.PF("1993/11/13 Programed By Yuzo Koshiro");
        }

        private static int AnalyzeOption(string[] args,ref int? seed)
        {
            int i = 0;
            int s = 0;

            while (args[i] != null && args[i].Length > 0 && args[i][0] == '-')
            {
                string op = args[i].Substring(1).ToUpper();
                if (op.Length > 2 && op.Substring(0, 2) == "SEED=")
                {
                    if (!int.TryParse(op.Substring(2), out s))
                    {
                        seed = null;
                    }
                    else seed = s;
                }

                i++;
            }

            return i;
        }

    }
}
