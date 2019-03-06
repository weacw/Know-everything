

using System.Collections.Generic;

public class Baike_info 
{
    public string baike_url;
    public string image_url;
    public string description;
}



public class ResultItem
{
    public double score;
    public string root;
    public Baike_info baike_info;
    public string keyword;
}


public class Result
{
    public long log_id;
    public int result_num;
    public List<ResultItem> result;
}