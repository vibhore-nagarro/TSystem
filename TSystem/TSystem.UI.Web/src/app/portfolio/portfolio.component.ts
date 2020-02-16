import { Component, OnInit, OnDestroy } from '@angular/core';
import { IPortfolioHolding, IPortfolioInfo } from '../core/services/models';
import { PortfolioService } from './portfolio.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss']
})
export class PortfolioComponent implements OnInit, OnDestroy {

  columns = ['Symbol', 'Qty', 'Avg. Price', 'LTP', 'P&L', 'Net. chg', 'Day. chg'];
  allHoldings: IPortfolioHolding[] = [];
  holdings: IPortfolioHolding[] = [];
  allPortfolios: IPortfolioInfo[] = [];
  selectedPortfolio: IPortfolioInfo;
  private subscriptions: Subscription[] = [];
  constructor(private portfolioService: PortfolioService) {
  }

  ngOnInit() {
    this.subscriptions.push(this.portfolioService.getAllHoldings().subscribe(data => {
      this.allHoldings = data;
      this.allHoldings.forEach(item => {
        item.netChange = (((item.lastPrice - item.averagePrice) / item.averagePrice) * 100);
        item.dayChange = item.netChange;
      });
      this.holdings = this.allHoldings;
    }));

    this.subscriptions.push(this.portfolioService.getAllPortfolios().subscribe(data => {
      this.allPortfolios = data;
      this.selectedPortfolio = this.allPortfolios[0];
    }));
  }

  onPortfolioChange(newValue: IPortfolioInfo) {
    this.subscriptions.push(this.portfolioService.getPortfolioHoldings(newValue.id).subscribe(data => {
      data.forEach(item => {
        item.netChange = (((item.lastPrice - item.averagePrice) / item.averagePrice) * 100);
        item.dayChange = item.netChange;
      });
      this.holdings = data;
    }));
  }

  ngOnDestroy() {
    if (this.subscriptions) {
      this.subscriptions.forEach(subscription => {
        subscription.unsubscribe();
      });
    }
  }
}
