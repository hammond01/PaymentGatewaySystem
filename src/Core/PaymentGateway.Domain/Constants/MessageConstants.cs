﻿namespace PaymentGateway.Domain.Constants;

public class MessageConstants
{
    public const string InternalServerError = "Internal Server Error";
}

public class MessageConstantsWithValue
{
    public static string createSuccess(string serviceName) => $"Create new {serviceName} successfully";
    public static string createFail(string serviceName, string errorMessage) => $"Create new {serviceName} fail: {errorMessage}";
    public static string updateSuccess(string serviceName) => $"{serviceName} has been updated successfully";
    public static string updateFail(string serviceName, string errorMessage) => $"{serviceName} has been updated fail: {errorMessage}";
    public static string getDataSuccess(string serviceName) => $"Get {serviceName} successfully";
    public static string getDataFail(string serviceName, string errorMessage) => $"Get {serviceName} fail: {errorMessage}";
    public static string notFoundFromDatabase(string serviceName) => $"This {serviceName} not found from database";
    public static string deleteSuccess(string serviceName) => $"Delete {serviceName} successfully";
    public static string deleteFail(string serviceName, string errorMessage) => $"Delete {serviceName} fail: {errorMessage}";
}