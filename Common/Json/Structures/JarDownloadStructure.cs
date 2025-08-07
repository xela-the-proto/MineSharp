namespace Common.Json.Structures;

public struct JarDownloadStructure
{
    public List<buildStructure> builds {get;set;}
}

//I should never acces this directly, since the compiler bitches about levels of access thi sis the solution
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public struct buildStructure
{
    public int id { get; set; }
    
    public string versionId { get; set; }
    
    public string projectVersionId { get; set; }
    
    public bool experimental { get; set; }
    
    public string jarUrl { get; set; }
    
    public int jarSize { get; set; }
}