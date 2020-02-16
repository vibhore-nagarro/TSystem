import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Api } from './api';
import { HttpClient } from '@angular/common/http';
import { IPortfolioHolding, IPortfolioInfo } from './models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  apiUrl = environment.apiUrl;

  AllHoldings = new Api<IPortfolioHolding>(this.http, this.apiUrl + 'portfolio/holdings');
  Portfolios = new Api<IPortfolioInfo>(this.http, this.apiUrl + 'portfolio');
  PortfolioHoldings = new Api<IPortfolioInfo>(this.http, this.apiUrl + 'portfolio/:portfolioId/holdings');
}
