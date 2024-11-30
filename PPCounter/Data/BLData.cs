namespace PPCounter.Data
{
    public class BLData
    {
        public string difficultyName { get; set; }
        public string modeName { get; set; }
        public float? stars { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public Modifiervalues modifierValues { get; set; }
        public Modifiersrating modifiersRating { get; set; }
        public float? passRating { get; set; }
        public float? accRating { get; set; }
        public float? techRating { get; set; }
    }

    public class Modifiervalues
    {
        public int modifierId { get; set; }
        public float da { get; set; }
        public float fs { get; set; }
        public float sf { get; set; }
        public float ss { get; set; }
        public float gn { get; set; }
        public float na { get; set; }
        public float nb { get; set; }
        public float nf { get; set; }
        public float no { get; set; }
        public float pm { get; set; }
        public float sc { get; set; }
        public float sa { get; set; }
        public float op { get; set; }
        public float ez { get; set; }
        public float hd { get; set; }
        public float smc { get; set; }
        public float ohp { get; set; }
    }

    public class Modifiersrating
    {
        public int id { get; set; }
        public float ssPredictedAcc { get; set; }
        public float ssPassRating { get; set; }
        public float ssAccRating { get; set; }
        public float ssTechRating { get; set; }
        public float ssStars { get; set; }
        public float fsPredictedAcc { get; set; }
        public float fsPassRating { get; set; }
        public float fsAccRating { get; set; }
        public float fsTechRating { get; set; }
        public float fsStars { get; set; }
        public float sfPredictedAcc { get; set; }
        public float sfPassRating { get; set; }
        public float sfAccRating { get; set; }
        public float sfTechRating { get; set; }
        public float sfStars { get; set; }
        public float bfsPredictedAcc { get; set; }
        public float bfsPassRating { get; set; }
        public float bfsAccRating { get; set; }
        public float bfsTechRating { get; set; }
        public float bfsStars { get; set; }
        public float bsfPredictedAcc { get; set; }
        public float bsfPassRating { get; set; }
        public float bsfAccRating { get; set; }
        public float bsfTechRating { get; set; }
        public float bsfStars { get; set; }
    }
}
