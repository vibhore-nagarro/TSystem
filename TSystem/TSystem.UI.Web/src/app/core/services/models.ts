export interface IApiResponse {
    data?: any;
    Data?: any;
    metadata?: any;
    MetaData?: any;
    result?: any;
    ErrorCode?: any;
}

export interface IPortfolioHolding {
    tradingsymbol: string;
    exchange: string;
    isin: string;
    product: string;
    quantity: number;
    averagePrice: number;
    lastPrice: number;
    pnL: number;
    netChange: number;
    dayChange: number;
}

export interface IPortfolioInfo {
    id: string;
    name: string;
}

export interface IPortfolio extends IPortfolioInfo {
    holdings: IPortfolioHolding[];
}

export class Portfolio implements IPortfolio {
    id: string;
    name: string;
    holdings: IPortfolioHolding[] = [];
}
