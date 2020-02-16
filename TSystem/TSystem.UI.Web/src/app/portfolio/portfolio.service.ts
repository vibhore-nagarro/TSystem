import { Injectable } from '@angular/core';
import { ApiService } from '../core/services/api.service';
import { map } from 'rxjs/operators';
import { IPortfolioHolding, IPortfolioInfo } from '../core/services/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {

  constructor(private apiService: ApiService) { }

  public getAllHoldings(): Observable<IPortfolioHolding[]> {
    return this.apiService.AllHoldings.getAll().pipe(map(response => {
      return response as IPortfolioHolding[];
    }));
  }

  public getAllPortfolios(): Observable<IPortfolioInfo[]> {
    return this.apiService.Portfolios.getAll().pipe(map(response => {
      return response as IPortfolioInfo[];
    }));
  }

  public getPortfolioHoldings(portfolioId: string): Observable<IPortfolioHolding[]> {
    return this.apiService.PortfolioHoldings.getAll({}, { portfolioId}).pipe(map(response => {
      return response as IPortfolioHolding[];
    }));
  }
}
