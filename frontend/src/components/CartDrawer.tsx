import type { CartItem } from "../types/commerce";

type CartDrawerProps = {
  isOpen: boolean;
  items: CartItem[];
  onClose: () => void;
  onRemove: (productId: string) => void;
  onIncreaseQuantity: (productId: string) => void;
  onDecreaseQuantity: (productId: string) => void;
  onSetQuantity: (productId: string, quantity: number) => void;
  onShopNow: () => void;
  onCheckout: () => void;
};

function getPriceValue(price: string) {
  return Number(price.replace(/[^0-9.]/g, "")) || 0;
}

export function CartDrawer({
  isOpen,
  items,
  onClose,
  onRemove,
  onIncreaseQuantity,
  onDecreaseQuantity,
  onShopNow,
  onCheckout,
}: CartDrawerProps) {
  const itemCount = items.reduce((total, item) => total + item.quantity, 0);
  const subtotal = items.reduce((total, item) => total + getPriceValue(item.product.price) * item.quantity, 0);

  return (
    <div className={isOpen ? "pointer-events-auto" : "pointer-events-none"} aria-hidden={!isOpen}>
      <div className={`cart-overlay ${isOpen ? "opacity-100" : "opacity-0"}`} onClick={onClose} />
      <aside
        className={`cart-panel ${isOpen ? "translate-x-0" : "translate-x-full"}`}
        aria-label="Shopping cart"
      >
        <div className="cart-header">
          <h2 className="cart-title">Cart</h2>
          <button className="close-cart" type="button" onClick={onClose}>
            Close
          </button>
        </div>
        {itemCount === 0 ? (
          <div className="cart-empty">
            <p>Your cart is empty.</p>
            <span>Discover our latest furniture pieces.</span>
            <button type="button" onClick={onShopNow}>
              Shop now
            </button>
          </div>
        ) : (
          <div className="cart-items">
            {items.map((item) => (
              <article key={item.product.id} className="cart-item">
                <img src={item.product.image} alt={item.product.name} />
                <div>
                  <h3>{item.product.name}</h3>
                  <p>{item.product.price}</p>
                  <button type="button" onClick={() => onRemove(item.product.id)}>
                    Remove
                  </button>
                </div>
                <div className="cart-quantity-stepper" aria-label={`${item.product.name} quantity`}>
                  <button type="button" onClick={() => onDecreaseQuantity(item.product.id)} aria-label="Decrease quantity">
                    -
                  </button>
                  <span>{item.quantity}</span>
                  <button type="button" onClick={() => onIncreaseQuantity(item.product.id)} aria-label="Increase quantity">
                    +
                  </button>
                </div>
              </article>
            ))}
          </div>
        )}
        {itemCount > 0 ? (
          <footer className="cart-footer">
            <div>
              <span>Subtotal</span>
              <span>$ {subtotal.toFixed(2)} USD</span>
            </div>
            <button type="button" onClick={onCheckout}>Continue to checkout</button>
          </footer>
        ) : null}
      </aside>
    </div>
  );
}
