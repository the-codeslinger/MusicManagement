namespace MusicManagementCore.Model
{
    /// <summary>
    /// Domain object representing the Cover.jpg file of an audio disc.
    /// </summary>
    public class CoverArt
    {
        public string Filename { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
