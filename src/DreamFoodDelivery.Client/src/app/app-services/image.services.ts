﻿/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.4.2.0 (NJsonSchema v10.1.11.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

@Injectable()
export class ImageModifiedService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "https://localhost:44318";
    }

    uploadImage(image: File | null | undefined, dishId: string | null | undefined): Observable<string> {
        let url_ = this.baseUrl + "/api/Image/upload?";
        if (dishId !== undefined)
            url_ += "DishId=" + encodeURIComponent("" + dishId) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = new FormData();
        if (image !== null && image !== undefined)
            content_.append("Image", image);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processUploadImage(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processUploadImage(<any>response_);
                } catch (e) {
                    return <Observable<string>><any>_observableThrow(e);
                }
            } else
                return <Observable<string>><any>_observableThrow(response_);
        }));
    }

    protected processUploadImage(response: HttpResponseBase): Observable<string> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return _observableOf(this.baseUrl + "/" + result200);
            }));
        } else if (status === 206) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result206: any = null;
            let resultData206 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result206 = ProblemDetails.fromJS(resultData206);
            return throwException("A server side error occurred.", status, _responseText, _headers, result206);
            }));
        } else if (status === 400) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = ProblemDetails.fromJS(resultData400);
            return throwException("A server side error occurred.", status, _responseText, _headers, result400);
            }));
        } else if (status === 403) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result403: any = null;
            let resultData403 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result403 = ProblemDetails.fromJS(resultData403);
            return throwException("A server side error occurred.", status, _responseText, _headers, result403);
            }));
        } else if (status === 500) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result500: any = null;
            let resultData500 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result500 = CustumResult.fromJS(resultData500);
            return throwException("A server side error occurred.", status, _responseText, _headers, result500);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<string>(<any>null);
    }

    getImage(dishId: string | null, imageName: string | null): Observable<string> {
        let url_ = this.baseUrl + "/api/Image/image/{dishId}/{imageName}";
        if (dishId === undefined || dishId === null)
            throw new Error("The parameter 'dishId' must be defined.");
        url_ = url_.replace("{dishId}", encodeURIComponent("" + dishId));
        if (imageName === undefined || imageName === null)
            throw new Error("The parameter 'imageName' must be defined.");
        url_ = url_.replace("{imageName}", encodeURIComponent("" + imageName));
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetImage(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetImage(<any>response_);
                } catch (e) {
                    return <Observable<string>><any>_observableThrow(e);
                }
            } else
                return <Observable<string>><any>_observableThrow(response_);
        }));
    }

    protected processGetImage(response: HttpResponseBase): Observable<string> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return _observableOf(this.baseUrl + "/" + result200);
            }));
          } else if (status === 206) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result206: any = null;
            let resultData206 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result206 = ProblemDetails.fromJS(resultData206);
            return throwException("A server side error occurred.", status, _responseText, _headers, result206);
            }));
        } else if (status === 400) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = ProblemDetails.fromJS(resultData400);
            return throwException("A server side error occurred.", status, _responseText, _headers, result400);
            }));
        } else if (status === 403) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result403: any = null;
            let resultData403 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result403 = ProblemDetails.fromJS(resultData403);
            return throwException("A server side error occurred.", status, _responseText, _headers, result403);
            }));
        } else if (status === 500) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result500: any = null;
            let resultData500 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result500 = CustumResult.fromJS(resultData500);
            return throwException("A server side error occurred.", status, _responseText, _headers, result500);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<string>(<any>null);
    }

    delete(dishId: string | null, imageName: string | null): Observable<void> {
        let url_ = this.baseUrl + "/api/Image/image/{dishId}/{imageName}";
        if (dishId === undefined || dishId === null)
            throw new Error("The parameter 'dishId' must be defined.");
        url_ = url_.replace("{dishId}", encodeURIComponent("" + dishId));
        if (imageName === undefined || imageName === null)
            throw new Error("The parameter 'imageName' must be defined.");
        url_ = url_.replace("{imageName}", encodeURIComponent("" + imageName));
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
            })
        };

        return this.http.request("delete", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processDelete(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processDelete(<any>response_);
                } catch (e) {
                    return <Observable<void>><any>_observableThrow(e);
                }
            } else
                return <Observable<void>><any>_observableThrow(response_);
        }));
    }

    protected processDelete(response: HttpResponseBase): Observable<void> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          return _observableOf<void>(<any>null);
          }));
      } else if (status === 206) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          let result206: any = null;
          let resultData206 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
          result206 = ProblemDetails.fromJS(resultData206);
          return throwException("A server side error occurred.", status, _responseText, _headers, result206);
          }));
      } else if (status === 400) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          let result400: any = null;
          let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
          result400 = ProblemDetails.fromJS(resultData400);
          return throwException("A server side error occurred.", status, _responseText, _headers, result400);
          }));
      } else if (status === 403) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          let result403: any = null;
          let resultData403 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
          result403 = ProblemDetails.fromJS(resultData403);
          return throwException("A server side error occurred.", status, _responseText, _headers, result403);
          }));
      } else if (status === 500) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          let result500: any = null;
          let resultData500 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
          result500 = CustumResult.fromJS(resultData500);
          return throwException("A server side error occurred.", status, _responseText, _headers, result500);
          }));
      } else if (status !== 200 && status !== 204) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          return throwException("An unexpected server error occurred.", status, _responseText, _headers);
          }));
      }
        return _observableOf<void>(<any>null);
    }

    getImageNamesList(dishId: string | null): Observable<string[]> {
        let url_ = this.baseUrl + "/api/Image/{dishId}";
        if (dishId === undefined || dishId === null)
            throw new Error("The parameter 'dishId' must be defined.");
        url_ = url_.replace("{dishId}", encodeURIComponent("" + dishId));
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetImageNamesList(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetImageNamesList(<any>response_);
                } catch (e) {
                    return <Observable<string[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<string[]>><any>_observableThrow(response_);
        }));
    }

    protected processGetImageNamesList(response: HttpResponseBase): Observable<string[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(item);
            }
            return _observableOf(result200);
            }));
          } else if (status === 206) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result206: any = null;
            let resultData206 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result206 = ProblemDetails.fromJS(resultData206);
            return throwException("A server side error occurred.", status, _responseText, _headers, result206);
            }));
        } else if (status === 400) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = ProblemDetails.fromJS(resultData400);
            return throwException("A server side error occurred.", status, _responseText, _headers, result400);
            }));
        } else if (status === 403) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result403: any = null;
            let resultData403 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result403 = ProblemDetails.fromJS(resultData403);
            return throwException("A server side error occurred.", status, _responseText, _headers, result403);
            }));
        } else if (status === 500) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result500: any = null;
            let resultData500 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result500 = CustumResult.fromJS(resultData500);
            return throwException("A server side error occurred.", status, _responseText, _headers, result500);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<string[]>(<any>null);
    }
}

export class ProblemDetails implements IProblemDetails {
    type?: string | null;
    title?: string | null;
    status?: number | null;
    detail?: string | null;
    instance?: string | null;
    extensions?: { [key: string]: any; } | null;

    constructor(data?: IProblemDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.type = _data["type"] !== undefined ? _data["type"] : <any>null;
            this.title = _data["title"] !== undefined ? _data["title"] : <any>null;
            this.status = _data["status"] !== undefined ? _data["status"] : <any>null;
            this.detail = _data["detail"] !== undefined ? _data["detail"] : <any>null;
            this.instance = _data["instance"] !== undefined ? _data["instance"] : <any>null;
            if (_data["extensions"]) {
                this.extensions = {} as any;
                for (let key in _data["extensions"]) {
                    if (_data["extensions"].hasOwnProperty(key))
                        this.extensions![key] = _data["extensions"][key];
                }
            }
        }
    }

    static fromJS(data: any): ProblemDetails {
        data = typeof data === 'object' ? data : {};
        let result = new ProblemDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["type"] = this.type !== undefined ? this.type : <any>null;
        data["title"] = this.title !== undefined ? this.title : <any>null;
        data["status"] = this.status !== undefined ? this.status : <any>null;
        data["detail"] = this.detail !== undefined ? this.detail : <any>null;
        data["instance"] = this.instance !== undefined ? this.instance : <any>null;
        if (this.extensions) {
            data["extensions"] = {};
            for (let key in this.extensions) {
                if (this.extensions.hasOwnProperty(key))
                    data["extensions"][key] = this.extensions[key] !== undefined ? this.extensions[key] : <any>null;
            }
        }
        return data;
    }
}

export interface IProblemDetails {
    type?: string | null;
    title?: string | null;
    status?: number | null;
    detail?: string | null;
    instance?: string | null;
    extensions?: { [key: string]: any; } | null;
}

export class CustumResult implements ICustumResult {
  message?: string | null;
  status?: number;

  constructor(data?: ICustumResult) {
      if (data) {
          for (var property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  init(_data?: any) {
      if (_data) {
          this.message = _data["message"] !== undefined ? _data["message"] : <any>null;
          this.status = _data["status"] !== undefined ? _data["status"] : <any>null;
      }
  }

  static fromJS(data: any): CustumResult {
      data = typeof data === 'object' ? data : {};
      let result = new CustumResult();
      result.init(data);
      return result;
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["message"] = this.message !== undefined ? this.message : <any>null;
      data["status"] = this.status !== undefined ? this.status : <any>null;
      return data;
  }
}

export interface ICustumResult {
  message?: string | null;
  status?: number;
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if (result !== null && result !== undefined)
        return _observableThrow(result);
    else
        return _observableThrow(new ApiException(message, status, response, headers, null));
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next("");
            observer.complete();
        } else {
            let reader = new FileReader();
            reader.onload = event => {
                observer.next((<any>event.target).result);
                observer.complete();
            };
            reader.readAsText(blob);
        }
    });
}
