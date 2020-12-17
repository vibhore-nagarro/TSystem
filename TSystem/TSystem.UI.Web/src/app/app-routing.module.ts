import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PortfolioComponent } from './portfolio/portfolio.component';
import { CreatePortfolioComponent } from './portfolio/create-portfolio/create-portfolio.component';


const routes: Routes = [
  { path: 'portfolio', component: PortfolioComponent },
  { path: 'portfolio/create', component: CreatePortfolioComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
