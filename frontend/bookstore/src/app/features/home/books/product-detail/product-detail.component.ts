import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { ProductService } from '../../../../services/product.service';
import { CartService } from '../../../../services/cart.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-product-detail',
  imports: [CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css'],
})
export class ProductDetailComponent implements OnInit {
  product: any = null; // Thông tin sản phẩm từ API
  isLoading = true; // Trạng thái tải dữ liệu
  errorMessage = ''; // Thông báo lỗi

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const bookId = Number(this.route.snapshot.paramMap.get('id'));

    this.productService.getProductById(bookId).subscribe({
      next: (data) => {
        this.product = {
          id: data.bookId,
          name: data.name,
          description: data.description,
          price: data.price,
          originalPrice: data.oldPrice,
          image: data.imageUrl,
          stock: data.stockQuantity,
        };
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Không thể tải thông tin sản phẩm. Vui lòng thử lại sau!';
        console.error(error);
        this.isLoading = false;
      },
    });
  }

  buyNow(): void {
    if (!this.authService.isLoggedIn()) {
      alert('Bạn cần đăng nhập để mua sản phẩm.');
      this.router.navigate(['/login']);
      return;
    }

    alert(`Bạn đã đặt mua "${this.product.name}" với giá ${this.product.price.toLocaleString()} VND.`);
    // Thêm logic xử lý đặt hàng nếu cần thiết
  }

  addToCart(): void {
    if (!this.authService.isLoggedIn()) {
      alert('Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.');
      this.router.navigate(['/login']);
      return;
    }

    const cartItem = {
      userId: this.authService.getCurrentUserId(),
      bookId: this.product.id,
      quantity: 1,
      addedAt: new Date().toISOString(),
      totalPrice: 0, // Có thể được xử lý phía server
      status: 'Added',
    };

    this.cartService.addCart(cartItem).subscribe({
      next: () => {
        alert('Sản phẩm đã được thêm vào giỏ hàng!');
      },
      error: (error) => {
        console.error('Không thể thêm sản phẩm vào giỏ hàng:', error);
        alert('Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau!');
      },
    });
  }
}
