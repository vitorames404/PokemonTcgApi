export interface CardWithPriceHistory {
  id: string;
  name: string;
  supertype: string | null;
  rarity: string | null;
  hp: number | null;
  image: string | null;
  artist: string | null;
  episodeName: string | null;
  episodeCode: string | null;
  tcgid: string | null;
  currentPrice: number | null;
  priceCurrency: string | null;
  priceHistory: PriceHistory[];
}

export interface PriceHistory {
  recordedAt: string;
  price: number;
  source: string;
}

export interface SearchResponse {
  data: CardWithPriceHistory[];
}
