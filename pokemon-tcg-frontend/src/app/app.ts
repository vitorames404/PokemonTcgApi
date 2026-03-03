import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardService } from './card.service';
import { CardWithPriceHistory } from './card.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  searchQuery = '';
  cards = signal<CardWithPriceHistory[]>([]);
  loading = signal(false);
  error = signal('');

  constructor(private cardService: CardService) {}

  search() {
    if (!this.searchQuery.trim()) return;

    this.loading.set(true);
    this.error.set('');
    this.cards.set([]);

    this.cardService.searchCards(this.searchQuery).subscribe({
      next: (response) => {
        this.cards.set(response.data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to search cards. Make sure the backend is running.');
        this.loading.set(false);
        console.error(err);
      }
    });
  }
}
