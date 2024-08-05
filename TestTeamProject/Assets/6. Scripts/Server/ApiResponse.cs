using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApiResponse
{
    public int status;
    public string message;
    public object date;
    public List<FieldError> errors;
}

[System.Serializable]
public class FieldError
{
    public string field;
    public string value;
}