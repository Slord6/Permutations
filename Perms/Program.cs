using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Perms
{
    class Program
    {
        private static char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static char[] ignore;
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Format = perms.exe \"??al??\" \"exclusions\"");
                return;
            }
            string input = args[0];
            if (args.Length > 1)
            {
                ignore = args[1].ToCharArray();
            } else
            {
                ignore = new char[0];
            }
            Console.WriteLine("Input: " + Environment.NewLine + input);
            Console.WriteLine("Ignoring: ");
            PrintArr(ignore);
            Console.WriteLine();

            char[] mask = input.ToCharArray();
            char[] currentWord = mask.ToArray();

            List<char> endPermBuilder = new List<char>();
            List<char> startPermBuilder = new List<char>();
            UInt64 lastFixedPosition = 0;
            for (int i = 0; i < currentWord.Length; i++)
            {
                startPermBuilder.Add('a');
                endPermBuilder.Add('z');
                if (currentWord[i] != '?') lastFixedPosition = (UInt64)i;
            }

            if (startPermBuilder.Count > 1)
            {
                startPermBuilder.Remove('a');
                startPermBuilder.Insert(0, 'b');
            }
            string startPerm = string.Join("", startPermBuilder);
            UInt64 counterStart = StringToInt(startPerm, alphabet);
            char[] endPerm = endPermBuilder.ToArray();
            UInt64 counter = counterStart;
            UInt64 increaseAmount = ColumnValue((UInt64)alphabet.Length, lastFixedPosition);

            HashSet<UInt64> previousValues = new HashSet<UInt64>();
            char[] currentPermutation = IntToString(counter, alphabet).ToCharArray();
            do
            {
                int permPosition = 0;
                for (int wordIndex = 0; wordIndex < currentWord.Length; wordIndex++)
                {
                    if (mask[wordIndex] == '?') // use generated letter
                    {
                        currentWord[wordIndex] = currentPermutation[permPosition];
                        permPosition++;
                    }
                    else
                    {
                        currentWord[wordIndex] = mask[wordIndex];
                        permPosition++;
                    }
                }

                UInt64 resultValue = StringToInt(string.Join("", currentWord), alphabet);
                if (previousValues.Contains(resultValue))
                {
                    counter += increaseAmount;
                    currentPermutation = IntToString(counter, alphabet).ToCharArray();
                    continue;
                }
                else
                {
                    previousValues.Add(resultValue);
                }
                if (IsValidWord(currentWord)) PrintArr(currentWord);

                counter++;
                currentPermutation = IntToString(counter, alphabet).ToCharArray();
            } while (!ArrayMatch(endPerm, currentPermutation));

        }

        private static bool IsValidWord(char[] wordLetters)
        {
            return ContainsVowel(wordLetters)
                && NotIgnorable(wordLetters)
                && ValidEndLetter(wordLetters[wordLetters.Length - 1])
                && ValidBigrams(wordLetters)
                && ValidEndSequence(wordLetters)
                && ContainsLikelyTrigrams(wordLetters)
                && ContainsLikelyNgramsStrict(wordLetters)
                && ValidInititalBigrams(wordLetters);
        }

        private static bool NotIgnorable(char[] wordLetters)
        {
            for (int i = 0; i < ignore.Length; i++)
            {
                if (wordLetters.Contains(ignore[i])) return false;
            }
            return true;
        }

        private static bool ValidEndLetter(char letter)
        {
            char[] valid = new char[] { 'i', 'u', 'v', 'j' };
            return !valid.Contains(letter);
        }

        private static bool ContainsLikelyTrigrams(char[] word)
        {
            string[] unlikelyTrigrams = new string[] {
                "zxk", "zvk", "zjk", "zjf", "xrq", "xpz", "xnq", "xkj", "vzy", "vvq", "vcq", "uqz", "uqy", "qzh", "qze",
                "qkk", "qjm", "qgk", "qgf", "qfq", "jzp", "jxb", "jcx", "ijq", "hvq", "fzj", "fqj", "cxq", "cwz", "bqv",
                "zqw", "znx", "zhx", "xzb", "vzz", "vwq", "vqo", "vmz", "vjy", "szq", "qzb", "qqj", "qnz", "qjt", "qjr",
                "qfk", "qdx", "qbx", "pqk", "pqj", "pjq", "jxv", "jqt", "jqs", "jnx", "jmq", "jkq", "jbz", "zqc", "xqh",
                "xkq", "wxz", "vxk", "vqm", "rqz", "qzr", "qwn", "qtz", "qkt", "qkq", "qkj", "qjp", "qjj", "qhq", "qck",
                "oqx", "mqz", "kzq", "kvq", "kqz", "kqy", "jtq", "jqy", "jqj", "fqz", "czq", "bwq", "bqz", "zwx", "zqt",
                "zqp", "zqk", "zqf", "znq", "zjv", "zgk", "zfj", "zbx", "xzd", "xqp", "xqg", "xlq", "wqj", "vvx", "vrq",
                "vjz", "qzc", "qyw", "qyc", "qxv", "qxk", "qwv", "qvk", "qsx", "qrg", "qpv", "qnj", "qhg", "qgb", "pwz",
                "oqz", "mqy", "kjx", "jwz", "jqv", "jqe", "jnq", "iyx", "hjx", "gqj", "cqz", "bgq", "zxj", "zqx", "zgj",
                "zdq", "yzq", "xzg", "xoq", "xjy", "xgq", "wxq", "vjx", "qzn", "qzl", "qwd", "qvu", "qrk", "qoj", "qnq",
                "qky", "qkg", "qjx", "qjl", "qdv", "pqz", "mqk", "kxz", "kqg", "jqd", "jhx", "bjx", "zxq", "ztq", "zqr",
                "zqj", "zqh", "zpq", "zlq", "zjy", "zcq", "zbq", "xvz", "xqn", "xlz", "wvz", "vxz", "vwz", "vkq", "tqz",
                "qvj", "qqy", "qqx", "qkp", "qkd", "qgq", "qgn", "qcz", "jxg", "jqo", "iwz", "iwq", "ijx", "cgz", "zqz",
                "zqq", "zqm", "zql", "zkx", "zjx", "zgx", "zcx", "yyq", "xzk", "xvq", "xmq", "xjq", "xhj", "wqz", "wqg",
                "vkx", "vbq", "uzx", "qzz", "qzu", "qyy", "qyl", "qvq", "qsz", "qqz", "qpy", "qmk", "qjq", "qjf", "qjc",
                "qfv", "pzj", "nqz", "lqx", "jzq", "jyz", "jwx", "jwq", "jvz", "jvx", "jqq", "jqp", "jlq", "jjx", "jfz",
                "gqz", "bvq", "zvx", "zrq", "zqd", "yqz", "xwz", "xqk", "xqe", "xmz", "xkz", "xgj", "xbz", "vzj", "vqg",
                "vjq", "qzx", "qzq", "qzp", "qzk", "qyq", "qwj", "qvx", "qgj", "mqj", "kxq", "jxq", "jxk", "jqx", "jlz",
                "jgx", "jgq", "hqz", "gzq", "fzq", "fjx", "dqz", "bzq", "bkq", "zqy", "zqn", "zmq", "zgq", "xzv", "xqz",
                "vqx", "vqk", "vfz", "uwx", "uqx", "ujq", "qzy", "qzv", "qzm", "qzg", "qzf", "qyx", "qxu", "qrj", "qmz",
                "qjy", "qjv", "qjk", "qhx", "qgy", "qgg", "qfj", "qcx", "pzq", "jyx", "jqz", "jqf", "jhz", "jfq", "gqk",
                "bxq", "zvq", "zqg", "zkq", "zjq", "zfq", "xwq", "xqj", "wzq", "vqj", "vgq", "vfq", "qzd", "qyv", "qyj",
                "qyg", "qxj", "qvz", "qpz", "qpj", "qkz", "qkx", "qgz", "qgx", "qgv", "qdz", "jzx", "jxz", "jql", "jqg", };

            for (int i = 0; i < word.Length - 2; i++)
            {
                string val = "";
                val += word[i];
                val += word[i + 1];
                val += word[i + 2];
                if (unlikelyTrigrams.Contains(val))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ContainsLikelyNgramsStrict(char[] word)
        {
            string[] ngrams = new string[]
            {
                "yjz","vjr","vjm","txk","pzw","lvx","kgv","hfj","fjz","bxo","zld","zcm","yzx","yyg","xfn","xdf","sqy",
                "mzl","gzr","eqx","yqm","vkp","vhz","uwy","tlx","klx","ivz","dvz","dhx","cxv","zxw","znr","uwv","qoc",
                "qeq","pzb","pxy","krq","fgx","cpx","cgx","zwt","zwd","zcs","xlx","vvj","qnu","qko","qcf","lqh","kmz",
                "jrq","jgf","hpx","bzm","yqp","xvd","vxr","qnw","qgl","pwv","mzp","mbz","jdn","hmz","gqe","zdp","xxk",
                "xmn","xdg","vxm","vvp","vmx","szj","sgz","quv","qtp","mvk","jyd","fzp","frq","zdx","yhx","xnm","xgn",
                "xcg","wjx","vxp","tmq","tkx","qnf","qlm","qfs","fpq","bzc","bgk","zpb","zlk","xhr","xbh","vvb","qtl",
                "qkn","pfx","lqm","lnq","jwy","jdv","bzy","zwc","zmp","yyz","ybz","xvn","wqs","wjn","vtz","qty","mlz",
                "kdz","jzt","jvd","jfb","dgq","zwy","whz","vnj","uvk","quz","jky","hzz","gzc","gvk","fqx","fqr","czv",
                "cqo","bwu","xrl","xbn","qmw","kxy","kpx","jhn","hfx","fyx","fpz","eqk","ejq","bzz","wfy","vwl","pgx",
                "jwl","fxk","bgj","zpg","zpf","zox","vxb","vpq","ujf","qns","qep","mgz","kqr","gzf","zmf","zhf","yqe",
                "ykz","xrk","xjc","vxj","vxd","sjz","qmm","lqr","kzt","kbv","jxt","jwp","jhh","jgp","hvz","gqc","xwu",
                "xjt","wzs","vxe","tjz","pzm","mzg","lzx","kxd","hhq","dwq","zcf","xhb","xez","vgv","tlq","qmp","pjn",
                "mjy","kzm","jwd","jbv","hzx","cqr","cfq","yyh","ywx","xkt","xbx","wxd","vvf","szx","obx","nbx","lbq",
                "jpv","jpk","jjv","jhd","dwz","bjk","zdh","ylx","xhc","wqx","slq","rvq","qsq","pnx","pdz","mqo","jdz",
                "gzl","fwx","fqd","bhj","xpk","tqn","qza","qow","hhz","dmq","bqh","ypz","xws","tbq","sgq","qrp","qgu",
                "lrz","kzy","iyz","gpq","gmq","bkj","zxc","xmk","xkp","vtj","vrj","uyx","qmf","qlr","qeu","oqn","mzz",
                "lqg","jzz","jkj","gzm","ggx","zdl","ylq","xfh","vkb","tbx","nqg","kjz","ivq","bvn","zgb","ycq","xvg",
                "xnr","wvk","wgk","vtx","qfn","pvy","kzr","jyp","jxx","jhk","jfy","iqv","zvu","zkc","wqt","vfv","uqh",
                "ujl","qxi","quj","qoz","nmq","mbx","lvq","jzo","dfz","zdm","vzu","vxo","twq","qax","pqn","kxj","jvv",
                "jkv","gwj","fvj","fqf","dxz","cxn","bqd","bdx","aqz","zdj","yyx","yrx","vkd","vhv","qtm","pxg","pjg",
                "nqy","lzj","lxk","kxu","klq","jsq","gzn","btz","zvy","zpd","znl","zbm","yqr","rqg","pzr","lvz","lqf",
                "kxg","jjz","fkx","cnz","cgk","bzb","bxh","blz","zqa","yqh","xlg","xcx","wqm","wqh","uqr","snq","qsv",
                "qnt","pxu","lbx","fvx","dkq","bzn","bxd","bwn","xnj","xkh","xjb","xgw","wxh","wkz","qrd","qht","qey",
                "nbq","kzp","gzp","gqp","zgt","yxj","yqf","wzr","wqw","vxh","vvn","vkf","vbv","qxt","qvt","qef","nqq",
                "lkx","kxh","hxg","cqf","bxl","bxe","zkb","zgy","yxz","ywq","yql","wjg","whj","vyz","vvg","tgz","qlh",
                "lzz","lcq","kfg","jfd","hzq","hvk","hqg","zgn","zbt","xwp","xqa","xlk","xjp","wmx","vwn","ubx","jyi",
                "jyf","gbz","cnk","wzd","wfv","vrz","uxz","qrm","pqo","mzv","kzk","jmj","hrx","bqe","zdg","xoj","xnl",
                "xgd","vzs","vjf","txj","sqg","qoh","qln","ljy","iyj","iqj","hcq","fql","clz","zxy","zvr","znz","zgw",
                "xwt","vqs","rxq","mqr","kzf","jlj","jgg","ikx","hwx","hvx","fvk","ztg","zbn","xzu","wxn","tnq","qnl",
                "pqd","nlq","kwx","jgd","hxn","gqr","fgz","zwp","zrl","xdj","wzy","wkx","wfj","pwk","pqh","kqc","kkz",
                "jhg","hkz","fqn","eqj","cfz","zgp","xnp","wrq","wcq","vwj","vqr","kyx","kqt","jzu","jhf","jgj","iwj",
                "hbq","gvx","cvk","cdz","znn","zkf","zdv","yfv","xwy","xrv","xrg","wfk","vdx","sfq","qux","qoy","qdn",
                "pqm","mqh","kxn","joq","jnz","gql","ejx","dqq","cpz","xkb","xbk","wvy","wlz","tgx","rwq","qtf","pvk",
                "pfz","jtj","jkk","gzz","gcx","fdq","zzx","zgs","zcb","xvv","xhm","wzl","wjk","wbq","vzc","vqd","tqg",
                "qmg","lhz","gwx","fbx","bxr","zrg","xhn","wzv","ujv","tqy","rwz","qxr","qcn","kkx","kbq","jwk","jgn",
                "fxj","dlq","bfy","avq","ajq","zxm","zpy","zlg","xkd","xjw","xbv","wqr","rhx","qqw","qqo","qnc","qji",
                "kxk","kjy","zxl","zwb","zvn","xsz","xnn","wfq","wdx","vuq","qqc","qcy","qcq","puq","gtq","gqh","dqx",
                "cvy","bbx","yqv","vwu","svq","qxa","qrw","qrl","qds","oqv","mzj","gcq","dqg","cxy","zxp","zuq","ywz",
                "vzr","uqq","ugx","tfz","sxz","sxq","qqs","qeg","pzy","mzk","kqv","jym","jwu","izx","fbq","zrk","zbp",
                "zbf","yqd","ymq","xwf","xpj","xoz","wxu","vzt","uqc","tgq","rqx","rqq","qnh","qmh","nzq","mgx","kwq",
                "kcx","jhv","gfv","bql","zxf","zwm","zgf","xhw","wvj","vjw","vcz","twz","trq","tkq","qsj","qrr","qmb",
                "qdc","pxj","mxj","kqs","kqd","jxa","jvn","hvj","fwz","bxv","bdz","zvm","yzj","xzh","xwx","xgx","xgf",
                "wtz","vzv","rxz","qqt","qpw","qeo","pzz","pwj","oqq","mnx","kqj","kpz","ihq","fvz","ztj","zkt","zbd",
                "ycx","uhz","prq","mgq","lxj","kzz","gmz","zrz","znk","znj","zmg","zlj","yxk","xoy","xhg","vxl","vux",
                "vqi","sjq","qrn","gpz","ygz","xhf","wqo","vhg","vbk","tqq","qqb","qnm","qdm","pzf","pqe","pbz","lgx",
                "kzc","ibq","hgq","cxg","cmq","bxg","bqg","blx","zxb","zrv","xmv","wzn","wzf","wjv","vwy","vvk","vjp",
                "rjx","qtb","nxq","mtq","mqe","kqx","jyw","gqn","gqd","fwq","zgc","xwv","xhx","xbg","wgz","uzq","qxd",
                "qqr","qqp","qql","pzx","mzx","jfn","hcx","djx","crq","cqn","brx","zmk","yvz","xwc","vfn","ufx","qmn",
                "qdw","jwg","jfj","glz","fqv","cjy","bxn","bmz","zvl","zlz","zfd","yqn","wzc","wbx","uqn","qlv","qgm",
                "qbq","pzd","nfq","mvx","lqn","jzy","jzg","hgx","hbx","cvq","cjz","bjv","zvz","zpz","zlv","zfp","zfh",
                "zcw","zcv","zbz","zbw","xzm","xwd","xhl","wzp","wql","vzm","vjb","qoa","qml","kqm","kcq","jzw","jmv",
                "jlv","jgk","jfg","gzv","gkx","cwx","bzg","bvz","bgv","zxo","zff","zck","ybx","xjh","vxq","vlx","vkk",
                "vjh","sqj","qfd","jvj","jlx","jfx","hwz","hqk","hgz","gvj","gbq","cvz","bzk","bpz","zwu","zgd","zbv",
                "ykx","xkx","xjm","xdz","wzg","wmz","vfg","uvj","rqy","qug","qnr","qhy","qgp","pjv","mqd","jyk","jxi",
                "jwj","czz","bqn","bjy","bdq","zpk","zfn","yqq","ygq","ybq","xqi","vjv","vjg","vfj","uqp","udx","qlg",
                "pqq","mjz","lgz","kql","kdx","jwf","iqx","dvq","bzl","bfq","zxz","zcn","ykq","xvy","xkf","vpx","vkg",
                "vhk","qxg","quk","qrb","qnb","jyc","gzg","cqv","bwk","zxe","zgg","wgj","uql","ukx","qpt","qoe","qhr",
                "kqk","jzn","jzh","jyh","jqb","ihz","dxq","clq","zjc","zfy","yvx","xnv","xfg","wgx","vzd","vqp","vjn",
                "vjl","vhq","vgk","qxc","qtn","qfg","qdy","qdf","pxk","mfz","lwx","jzk","jzf","jrz","jgv","iwv","hkx",
                "gqq","fqg","dzj","bfz","zxh","ztk","zjs","xzw","xwn","wpq","wjy","tjq","pzn","jxp","jgy","hwq","hbz",
                "glq","fhx","zxx","zwn","ygx","wzj","vvz","vcx","ujg","rjq","qxf","qwm","qvw","qry","qpf","pqg","pgq",
                "njq","kqq","kqp","jvk","iqy","cqg","cmz","zvj","zdk","wqk","wqd","vpz","uuq","uqf","qys","qxw","qww",
                "qly","qdt","pzg","mhx","lhx","jzc","jgz","hjz","hhx","dfq","aqx","zxg","zwz","yrq","wvx","qhp","qfb",
                "qez","mwq","kqo","kgz","jzb","hqj","hkq","gxk","gxj","bxu","zwk","zwf","znv","zkk","zfw","xxz","xuq",
                "xqs","xjj","xgg","wxv","wxj","wvq","vbx","sqx","qzi","qyp","qxb","qvb","qks","pzv","pvj","ohx","mql",
                "mqg","mfq","kmq","kjq","jxs","eqz","dqy","bhz","bhq","zvd","zkg","zfb","xzn","xwl","xcj","wqp","vgj",
                "uux","txq","qvr","qmv","qkl","qdb","pvx","mvq","mdq","knq","jzr","jxh","jqc","jmx","grq","zvv","yyj",
                "xvj","xnk","vxu","tfq","qyd","qwu","qvp","qlz","qgh","qdh","oqk","oqg","kvz","kqh","jqk","jdq","fkq",
                "zkd","zjw","ytq","xpq","xjf","xcz","wfz","vzl","vwk","qzo","qyh","qxs","qws","qtg","qfw","qdq","lrq",
                "lgq","kkq","kgq","kdq","kbz","jyb","jxw","jqa","gdq","fzk","fnq","bpx","zwj","zmz","zfv","xzs","xzl",
                "xqr","xdk","xbq","wqn","vlz","uwq","qxx","qtk","qqm","qhd","pxz","pnq","pgz","ozx","lwz","gzk","gkz",
                "bvk","bpq","zvb","xzf","xqt","wqy","vzg","vxg","vqw","vql","txz","qyi","qwf","qpd","qox","qnp","qhm",
                "nqj","kwz","kqf","jwv","iqz","hqx","hfq","cwq","bqk","zxd","zvc","zpj","zkz","zjd","yxq","xzc","xwg",
                "xqw","wqf","vqv","sjx","rzq","qxe","qvf","qqv","qhw","pbq","mqn","gqg","zxu","ztx","zjh","yqg","xzx",
                "xjx","xhh","vkj","vhj","vfk","vdz","uqj","qqh","qqe","qpn","pkx","pjy","mkq","lqq","kzg","kbx","jzm",
                "jxd","jvy","jrx","jhj","zqi","zfg","yqy","xzp","xvk","xuu","xlj","xbj","vxy","vqt","vjj","vgx","qpk",
                "qnn","ozq","nqx","kvx","kfq","jzd","gwz","gqy","gfz","fzx","dqj","czx","cqx","bzv","zwg","zvt","zjt",
                "zbj","xjd","xdq","wyq","wqv","vzb","vqc","ufq","qwk","qmq","pvz","mjx","lqk","kzx","kpq","jxr","hzj",
                "hxz","hxj","gvz","dzq","bqj","zvh","zmv","zlx","zjm","zfk","xqy","xqb","xhq","xcq","xck","wyx","wpz",
                "wmq","qtx","qdj","qcj","mqq","jxm","bxy","bmq","zvw","ztz","zpx","zhj","xqv","xqc","xkk","xjg","xhk",
                "wtq","wqq","rqk","qxy","qxm","qxl","qtj","qrh","qmx","oqj","mxq","mvz","lwq","lqz","jzl","jyg","jxn",
                "jxl","jxf","jqh","hqy","gxz","gjz","bwx","bwj","brq","zux","zjj","zfz","xqf","xkv","vzp","uvx","tqk",
                "qwp","qqf","qkr","qhj","qgt","pwx","kzv","ktq","khx","jcq","hjq","gwq","cqq","bgz","zvp","zkv","zjn",
                "zfx","zbk","xzz","xzt","xzj","xjz","xjl","xjk","whx","wgq","vwx","vqe","vjk","vfy","vbz","qyu","qyb",
                "qxo","qwt","qwq","qwc","qtq","qrv","qpq","qlj","qjn","qhh","qhb","lxz","jtx","hxk","fxz","cqk","zcj",
                "zbg","yqx","yqk","xzy","xxq","xrz","xfv","xfk","wjq","vqb","uvz","ucx","qyt","qym","qxp","qvm","qqd",
                "qny","qjs","qfh","qdp","qdl","qdk","mxz","lqj","jqi","jmz","hqq","gjq","cgq","bvj","zxn","zrj","zjp",
                "zjb","zhq","xqx","xgv","wzx","vqq","qyz","qwy","qrx","qqk","qnv","qhl","qgc","pvq","pqx","pkq","oqy",
                "nqk","jzj","jyy","jxo","jxj","jpz","jpx","jpq","jkz","gzx","gkq","fxq","fvq","cxz","cxk","cxj","blq",
                "zqb","zpv","zjg","zgv","yqj","xhz","xgk","vqh","qyk","qvd","qpb","qlk","qkc","qjh","qhk","qfx","qdg",
                "qcg","pwq","mjq","lqy","ljx","jxe","jqr","jqm","iyq","hfz","gjx","cqy","bkz","bjz","ajx","zjz","zgz",
                "zcg","xql","xjn","xfq","uqg","tqx","qwl","qog","qlx","qhf","pxq","ojx","ojq","njx","mqx","kgx","jbx",
                "jbq","gvq","gfq","fqy","bzx","bxj","bvy","zvg","zjl","xwk","xwj","xqm","xnz","vzk","vtq","uwz","uqv",
                "tqj","qzt","qzs","qyf","qxn","qxh","qvh","qkb","pfq","jzv","jvq","jsx","jjq","jfv","fqq","dqk","bhx",
                "bgx","zxv","zvf","zqs","yfq","xzr","xhv","xgz","xfj","vmq","uqk","ujz","rqj","qyr","qqn","qpg","qnk",
                "qhn","qgw","qek","qcv","pjz","mzq","lxq","ljq","jxy","jqw","jqn","jkx","gxq","fjq","cqj","cjq","zwv",
                "zsx","zrx","zmx","zmj","zkj","xrj","xqo","xjv","xfz","vnx","vlq","vkz","uhx","qzw","qyn","qxq","qwx",
                "qwg","qwb","qvv","qvn","qpx","qkw","qkf","qjw","qjd","qej","kzj","jtz","jhq","jdx","ihx","gzj","gqx",
                "fqk","cjx","bzj","bxk","bwz","bvx","bjq","zqo","zqe","yvq","yjx","yjq","xqq","xqd","xmj","vqf","vgz",
                "uvq","sqz","qvl","qrz","qrq","qqg","qjb","pqy","pqv","pjx","kqn","jyq","jyj","jxu","iwx","hxq","bqy",
                "bqx","bqq","zxk","zvk","zjk","zjf","xrq","xpz","xnq","xkj","vzy","vvq","vcq","uqz","uqy","qzh","qze",
                "qkk","qjm","qgk","qgf","qfq","jzp","jxb","jcx","ijq","hvq","fzj","fqj","cxq","cwz","bqv","zq","zn","zh",
                "xz","vz","vw","vq","vm","vj","sz","qz","qq","qn","qj","qj","qf","qd","qb","pq","pq","pj","jx","jq","jq",
                "jn","jm","jk","jb","zq","xq","xk","wx","vx","vq","rq","qz","qw","qt","qk","qk","qk","qj","qj","qh","qc",
                "oq","mq","kz","kv","kq","kq","jt","jq","jq","fq","cz","bw","bq","zw","zq","zq","zq","zq","zn","zj","zg",
                "zf","zb","xz","xq","xq","xl","wq","vv","vr","vj","qz","qy","qy","qx","qx","qw","qv","qs","qr","qp","qn",
                "qh","qg","pw","oq","mq","kj","jw","jq","jq","jn","iy","hj","gq","cq","bg","zx","zq","zg","zd","yz","xz",
                "xo","xj","xg","wx","vj","qz","qz","qw","qv","qr","qo","qn","qk","qk","qj","qj","qd","pq","mq","kx","kq",
                "jq","jh","bj","zx","zt","zq","zq","zq","zp","zl","zj","zc","zb","xv","xq","xl","wv","vx","vw","vk","tq",
                "qv","qq","qq","qk","qk","qg","qg","qc","jx","jq","iw","iw","ij","cg","zq","zq","zq","zq","zk","zj","zg",
                "zc","yy","xz","xv","xm","xj","xh","wq","wq","vk","vb","uz","qz","qz","qy","qy","qv","qs","qq","qp","qm",
                "qj","qj","qj","qf","pz","nq","lq","jz","jy","jw","jw","jv","jv","jq","jq","jl","jj","jf","gq","bv","zv",
                "zr","zq","yq","xw","xq","xq","xm","xk","xg","xb","vz","vq","vj","qz","qz","qz","qz","qy","qw","qv","qg",
                "mq","kx","jx","jx","jq","jl","jg","jg","hq","gz","fz","fj","dq","bz","bk","zq","zq","zm","zg","xz","xq",
                "vq","vq","vf","uw","uq","uj","qz","qz","qz","qz","qz","qy","qx","qr","qm","qj","qj","qj","qh","qg","qg",
                "qf","qc","pz","jy","jq","jq","jh","jf","gq","bx","zv","zq","zk","zj","zf","xw","xq","wz","vq","vg","vf",
                "qz","qy","qy","qy","qx","qv","qp","qp","qk","qk","qg","qg","qg","qd","jz","jx","jq","jq"
            };

            for (int i = 0; i < word.Length - 2; i++)
            {
                string val = "";
                val += word[i];
                val += word[i + 1];
                val += word[i + 2];
                if (ngrams.Contains(val) || ngrams.Contains(val.Remove(2)))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidInititalBigrams(char[] word)
        {
            if (word.Length < 2) return true;
            string[] initialBigrams = new string[] { 
                "bx", "cg", "fb", "fr", "fp", "fv", "fx", "he", "hg", "hx",
                "jq", "jx", "pl", "pq", "px", "qg", "qj", "qk", "qp", "vq",
                "wf", "wq", "xd", "xi", "xj", "xz", "zp"
            };
            int end = Math.Min(word.Length - 1, 2);
            
            char[] bigram = new char[] { word[0], word[1] };
            if (initialBigrams.Contains(new string(bigram)))
            {
                return false;
            }

            return true;
        }

        private static bool ValidBigrams(char[] word)
        {
            // This isn't strictly accurate, but think it wittles out the least likely
            // Think correct is JQ, QG, QK, QY, QZ, WQ, and WZ (http://norvig.com/mayzner.html)
            string[] invalidBigrams = new string[] {
                "bk", "fq", "jc", "jt", "mj", "qh", "qx", "vj", "wz", "zh",
                "bq", "fv", "jd", "jv", "mq", "qj", "qy", "vk", "xb", "zj",
                "bx", "fx", "jf", "jw", "mx", "qk", "qz", "vm", "xg", "zn",
                "cb", "fz", "jg", "jx", "mz", "ql", "sx", "vn", "xj", "zq",
                "cf", "gq", "jh", "jy", "pq", "qm", "sz", "vp", "xk", "zr",
                "cg", "gv", "jk", "jz", "pv", "qn", "tq", "vq", "xv", "zs",
                "cj", "gx", "jl", "kq", "px", "qo", "tx", "vt", "xz", "zx",
                "cp", "hk", "jm", "kv", "qb", "qp", "vb", "vw", "yq", "qi",
                "cv", "hv", "jn", "kx", "qc", "qr", "vc", "vx", "yv", "qa",
                "cw", "hx", "jp", "kz", "qd", "qs", "vd", "vz", "yz", "qo",
                "cx", "hz", "jq", "lq", "qe", "qt", "vf", "wq", "zb", "qe",
                "dx", "iy", "jr", "lx", "qf", "qv", "vg", "wv", "zc",
                "fk", "jb", "js", "mg", "qg", "qw", "vh", "wx", "zg" };

            for (int i = 0; i < word.Length - 1; i++)
            {
                string val = "";
                val += word[i];
                val += word[i + 1];
                if (invalidBigrams.Contains(val))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidEndSequence(char[] word)
        {
            // The suffix –FUL is always spelt with one L
            if (word.Length <= 5) return true;
            string joined = new string(word);
            if (joined.EndsWith("full")) return false;

            return true;
        }

        private static bool ContainsVowel(char[] letters)
        {
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            for (int i = 0; i < letters.Length; i++)
            {
                if (vowels.Contains(letters[i])) return true;
            }
            return false;
        }

        private static bool ArrayMatch(char[] a, char[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static void PrintArr(char[] arr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append(arr[i]);
            }
            Console.WriteLine(sb.ToString());
        }

        private static string IntToString(UInt64 value, char[] baseChars)
        {
            string result = string.Empty;
            UInt64 targetBase = (UInt64)baseChars.Length;

            do
            {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

        private static UInt64 StringToInt(string value, char[] baseChars)
        {
            UInt64 result = 0;
            UInt64 targetBase = (UInt64)baseChars.Length;
            char[] valueLetters = value.ToCharArray();

            for (int columnPos = 0; columnPos < valueLetters.Length; columnPos++)
            {
                UInt64 columnValue = ColumnValue(targetBase, (UInt64)(valueLetters.Length - columnPos));
                UInt64 letterValue = (UInt64)Array.IndexOf(baseChars, valueLetters[columnPos]);
                UInt64 score = columnValue * letterValue;
                result += score;
            }
             
            return (UInt64)result;
        }

        private static UInt64 ColumnValue(UInt64 targetBase, UInt64 positon)
        {
            return (UInt64)Math.Pow(targetBase, (positon - 1));
        }
    }
}
