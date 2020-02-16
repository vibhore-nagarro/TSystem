import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PortfolioComponent } from './portfolio/portfolio.component';
import { ApiService } from './core/services/api.service';

import { PortfolioService } from './portfolio/portfolio.service';
import { CreatePortfolioComponent } from './portfolio/create-portfolio/create-portfolio.component';

@NgModule({
  declarations: [
    AppComponent,
    PortfolioComponent,
    CreatePortfolioComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule
  ],
  exports: [
    HttpClientModule,
  ],
  providers: [
    ApiService,
    PortfolioService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
