import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchResponse } from './card.model';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  private apiUrl = 'http://localhost:5217/api/cards';

  constructor(private http: HttpClient) {}

  searchCards(name: string): Observable<SearchResponse> {
    return this.http.get<SearchResponse>(
      `${this.apiUrl}/search-with-history?name=${encodeURIComponent(name)}`
    );
  }
}