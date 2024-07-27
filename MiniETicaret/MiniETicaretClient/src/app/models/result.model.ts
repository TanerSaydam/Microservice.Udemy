export class ResultModel<T>{
    data?: T | null;
    errorMessages?: string[] | null;
    isSuccessful: boolean = false;
}