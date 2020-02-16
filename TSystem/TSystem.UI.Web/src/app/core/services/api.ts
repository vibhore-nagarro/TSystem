import { IApiResponse } from './models';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export class Api<T> {

   constructor(protected http: HttpClient, protected actionUrl: string, protected TModel?: new (...args) => T) {
   }

   get(id: number): Observable<IApiResponse> {
      const req = this.http.get<IApiResponse>(`${this.actionUrl}/${id}`);

      if (this.TModel) {
         return req.pipe(map(response => {
            const objTModel = new this.TModel();
            response.data = this.fromJson(objTModel, response.data);
            return response;
         }));
      }

      return req;
   }

   post(payLoad: any = {}): Observable<any> {
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
      return this.http.post(this.actionUrl, payLoad, {
         headers: headers
      });
   }

   getAll(query: any = {}, routeParams?: any): Observable<IApiResponse> {
      const queryParams = this.createQueryParams(query);
      const routeURL = this.createRouteURL(routeParams);

      const req = this.http.get<IApiResponse>(routeURL, {
         params: queryParams
      });

      if (this.TModel) {
         return req.pipe(map(response => {
            response.data = response.data.map(data => {
               const objTModel = new this.TModel();
               return this.fromJson(objTModel, data);
            });
            return response;
         }));
      }

      return req;
   }

    getBlobDownload(fileType: any, fileData: string): Observable<any> {
            return this.http.get(fileData, {
            responseType: 'blob',
         }).pipe(map((res) => {
            return new Blob([res], { type: fileType });
         }));
      }

    getBlob(fileType: any, query: any = {}): Observable<any> {
       const queryParams = this.createQueryParams(query);

       return this.http.get(this.actionUrl, {
          params: queryParams,
          responseType: 'blob',
       }).pipe(map((res) => {
          return new Blob([res], { type: fileType });
      }));
    }

   postGetBlob(fileType: any, payLoad: any = {}): Observable<any> {
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
      return this.http.post(this.actionUrl, payLoad, {
         headers: headers,
         responseType: 'blob'
      });
   }

   create(payload: T, query: any = {}, routeParams?: any) {
      const queryParams = this.createQueryParams(query);
      const routeURL = this.createRouteURL(routeParams);
      return this.http.post<IApiResponse>(routeURL, payload, {
         params: queryParams
      });
   }

   update(payload: T = <T>{}, query: any = {}, routeParams?: any) {
      const queryParams = this.createQueryParams(query);
      const routeURL = this.createRouteURL(routeParams);

      return this.http.put<IApiResponse>(routeURL, payload, {
         params: queryParams
      });
   }

   updateById(id, payload: T = <T>{}, query: any = {}) {
      const queryParams = this.createQueryParams(query);
      return this.http.put<IApiResponse>(`${this.actionUrl}/${id}`, payload, {
         params: queryParams
      });
   }

   remove(id, query: any = {}, payload?: T) {
      const queryParams = this.createQueryParams(query);
      const options = {
         params: queryParams,
         body: payload
      };
      return this.http.delete<IApiResponse>(`${this.actionUrl}/${id}`, options);
   }

   private fromJson(toObject, json: any): T {
      return Object.assign(toObject, json);
   }

   private createQueryParams(query: any): HttpParams {
      let params = new HttpParams();

      for (const key in query) {
         params = params.append(key, query[key]);
      }

      return params;
   }

   private createRouteURL(routeParams: any): string {
      let urlResult = this.actionUrl;
      if (routeParams) {
         for (const param in routeParams) {
            const myRegExp = new RegExp(':' + param);
            urlResult = urlResult.replace(myRegExp, routeParams[param]);
         }
      }
      return urlResult;
   }
}
