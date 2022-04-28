import { AxiosError } from "axios";

export interface IApiError {
    status: number,
    message: string,
    data: any,
}

export function ErrorConverter(value: AxiosError | any) : IApiError {
    if(value.response){
        return {
            status: value.response.status,
            message: value.response?.data?.message ?? value.response?.message,
            data: value.response.data
        }
    }
    else if(value.request){
        return {
            status: value.request.status,
            message: value.request?.message ?? "",
            data: value.request.data
        }
    }
    
    return {
        status: value?.status,
        message: value.message,
        data: value?.toJSON()
    }
}