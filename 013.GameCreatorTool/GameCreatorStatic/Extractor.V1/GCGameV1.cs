using System;

namespace GameCreatorStatic.Extractor.V1
{
    /// <summary>
    /// 《令和罕见物语》
    /// </summary>
    public class LingHeHanJianWuYv : GCExtractorV1
    {
        public override string Title => "令和罕见物语";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image;
        public override string Version => "0.9902";
    }


    /// <summary>
    /// 《叛军组织的我爱上了贵族大小姐》
    /// </summary>
    public class FellInLoveWithTheNobilityGirlAsAMemberOfTheRebelOrganization : GCExtractorV1
    {
        public override string Title => "叛军组织的我爱上了贵族大小姐";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image;
        public override string Version => "0.99072";
    }

    /// <summary>
    /// 《残神觉醒》
    /// </summary>
    public class BrokenGodAwakening : GCExtractorV1
    {
        public override string Title => "残神觉醒";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image | GCEntryptionFlagV1.Text;
        public override string Version => "0.99131";
        public override string TextKey => "gc_zip_2024";
    }

    /// <summary>
    /// 鼓手余命十日谭
    /// </summary>
    public class ShiinaTakisDecameron : GCExtractorV1
    {
        public override string Title => "鼓手余命十日谭";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image ;
        public override string Version => "0.99131";
    }

    /// <summary>
    /// 《在时间的尽头等你》
    /// </summary>
    public class WaitingForYouAtTheEndOfTime : GCExtractorV1
    {
        public override string Title => "在时间的尽头等你";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image | GCEntryptionFlagV1.Text;
        public override string Version => "0.9914";
        public override string TextKey => "gc_zip_2024";
    }

    /// <summary>
    /// 《我亲爱的妹妹》
    /// </summary>
    public class HappySistersLife : GCExtractorV1
    {
        public override string Title => "我亲爱的妹妹";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image;
        public override string Version => "0.9914";
    }

    /// <summary>
    /// 《风之歌》
    /// </summary>
    public class WindsPoem : GCExtractorV1
    {
        public override string Title => "风之歌";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image;
        public override string Version => "0.9914";
    }

    /// <summary>
    /// 《暮雨流花+》
    /// </summary>
    public class FloainPlus : GCExtractorV1
    {
        public override string Title => "暮雨流花+";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image | GCEntryptionFlagV1.Text | GCEntryptionFlagV1.Audio;
        public override string Version => "0.99161";
        public override string TextKey => "gc_zip_2024";
        public override string AudioKey => "gc_zip_2024";
    }

    /// <summary>
    /// 《重返大学时代》
    /// </summary>
    public class ReturnToCollegeAge : GCExtractorV1
    {
        public override string Title => "重返大学时代";
        public override GCEntryptionFlagV1 EntryptionFlag => GCEntryptionFlagV1.Image | GCEntryptionFlagV1.Text;
        public override string Version => "0.9917";
        public override string TextKey => "gc_zip_2024";
    }
}
