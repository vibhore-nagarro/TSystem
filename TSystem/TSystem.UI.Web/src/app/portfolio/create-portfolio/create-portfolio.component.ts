import { Component, OnInit, OnDestroy } from '@angular/core';
import { IPortfolioHolding, IPortfolio, Portfolio } from 'src/app/core/services/models';
import { Subscription } from 'rxjs';
import { PortfolioService } from '../portfolio.service';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-create-portfolio',
  templateUrl: './create-portfolio.component.html',
  styleUrls: ['./create-portfolio.component.scss']
})
export class CreatePortfolioComponent implements OnInit, OnDestroy {

  columns = ['Symbol', 'Qty', 'Avg. Price', 'LTP', 'P&L', 'Net. chg', 'Day. chg', ' '];
  allHoldings: IPortfolioHolding[] = [];
  private subscriptions: Subscription[] = [];
  portfolio: IPortfolio;

  constructor(private portfolioService: PortfolioService) { }

  ngOnInit() {
    this.subscriptions.push(this.portfolioService.getAllHoldings().subscribe(data => {
      this.allHoldings = data;
      this.allHoldings.forEach(item => {
        item.netChange = (((item.lastPrice - item.averagePrice) / item.averagePrice) * 100);
        item.dayChange = item.netChange;
      });
    }));

    this.portfolio = new Portfolio();
  }

  addToPortfolio(holding: IPortfolioHolding) {
    this.portfolio.holdings.push(holding);
  }

  removeFromPortfolio(holding: IPortfolioHolding) {
    this.portfolio.holdings.splice(this.portfolio.holdings.indexOf(holding), 1);
  }

  savePortfolio() {

  }

  ngOnDestroy() {
    if (this.subscriptions) {
      this.subscriptions.forEach(subscription => {
        subscription.unsubscribe();
      });
    }
  }
}
