import { useMemo, useState } from "react";
import type { ReactNode } from "react";
import { products } from "../data/landingData";
import type { CartItem, Product } from "../types/commerce";
import { ProductCard } from "./ProductCard";

type CheckoutPageProps = {
  items: CartItem[];
  onProductNavigate: (product: Product) => void;
  onShopNavigate: () => void;
};

type ShippingMethod = "standard" | "express";
type PaymentMethod = "card" | "apple" | "cash";

const shippingOptions: Array<{ id: ShippingMethod; title: string; price: number; displayPrice: string }> = [
  { id: "standard", title: "Standard Delivery", price: 0, displayPrice: "Free" },
  { id: "express", title: "Express Delivery", price: 18, displayPrice: "$ 18.00 USD" },
];

const paymentOptions: Array<{ id: PaymentMethod; title: string }> = [
  { id: "card", title: "Credit Card" },
  { id: "apple", title: "Apple Pay" },
  { id: "cash", title: "Cash on Delivery" },
];

function getPriceValue(price: string) {
  return Number(price.replace(/[^0-9.]/g, "")) || 0;
}

function formatCurrency(value: number) {
  return `$ ${value.toFixed(2)} USD`;
}

export function CheckoutPage({ items, onProductNavigate, onShopNavigate }: CheckoutPageProps) {
  const [shippingMethod, setShippingMethod] = useState<ShippingMethod>("standard");
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("card");
  const [promoCode, setPromoCode] = useState("");
  const [isPromoApplied, setIsPromoApplied] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const totals = useMemo(() => {
    const subtotal = items.reduce((total, item) => total + getPriceValue(item.product.price) * item.quantity, 0);
    const shipping = shippingOptions.find((option) => option.id === shippingMethod)?.price ?? 0;
    const discount = isPromoApplied ? subtotal * 0.1 : 0;
    const tax = (subtotal - discount) * 0.08;
    const total = subtotal - discount + shipping + tax;

    return { subtotal, shipping, discount, tax, total };
  }, [isPromoApplied, items, shippingMethod]);

  if (isSuccess) {
    return (
      <main id="top" className="checkout-page">
        <CheckoutSuccessState onShopNavigate={onShopNavigate} />
      </main>
    );
  }

  return (
    <main id="top" className="checkout-page">
      <CheckoutHero />
      {items.length === 0 ? (
        <CheckoutEmptyState onShopNavigate={onShopNavigate} />
      ) : (
        <section className="checkout-main" aria-label="Checkout">
          <div className="checkout-container checkout-grid">
            <CheckoutForm
              paymentMethod={paymentMethod}
              shippingMethod={shippingMethod}
              onPaymentMethodChange={setPaymentMethod}
              onShippingMethodChange={setShippingMethod}
            />
            <OrderSummary
              items={items}
              totals={totals}
              promoCode={promoCode}
              isPromoApplied={isPromoApplied}
              onPromoCodeChange={setPromoCode}
              onApplyPromo={() => setIsPromoApplied(promoCode.trim().length > 0)}
              onCompleteOrder={() => setIsSuccess(true)}
            />
          </div>
        </section>
      )}
      <RecommendedProducts onProductNavigate={onProductNavigate} />
    </main>
  );
}

export function CheckoutHero() {
  return (
    <section className="checkout-hero" aria-labelledby="checkout-title">
      <p>Secure luxury checkout experience.</p>
      <h1 id="checkout-title">Checkout</h1>
    </section>
  );
}

type CheckoutFormProps = {
  paymentMethod: PaymentMethod;
  shippingMethod: ShippingMethod;
  onPaymentMethodChange: (method: PaymentMethod) => void;
  onShippingMethodChange: (method: ShippingMethod) => void;
};

export function CheckoutForm({
  paymentMethod,
  shippingMethod,
  onPaymentMethodChange,
  onShippingMethodChange,
}: CheckoutFormProps) {
  return (
    <form className="checkout-form" onSubmit={(event) => event.preventDefault()}>
      <CheckoutSection title="Contact Information">
        <div className="checkout-field-grid two-columns">
          <CheckoutField label="Email" type="email" placeholder="atelier@example.com" />
          <CheckoutField label="Phone" type="tel" placeholder="+1 212 555 0198" />
        </div>
      </CheckoutSection>

      <CheckoutSection title="Shipping Address">
        <div className="checkout-field-grid two-columns">
          <CheckoutField label="First Name" placeholder="First name" />
          <CheckoutField label="Last Name" placeholder="Last name" />
          <CheckoutSelect label="Country" />
          <CheckoutField label="City" placeholder="City" />
          <CheckoutField label="Address Line" placeholder="Street and number" className="span-two" />
          <CheckoutField label="Apartment" placeholder="Apartment, suite, floor" />
          <CheckoutField label="Postal Code" placeholder="Postal code" />
          <CheckoutTextarea label="Notes" placeholder="Delivery details, access notes, or preferred timing" />
        </div>
      </CheckoutSection>

      <CheckoutSection title="Shipping Method">
        <ShippingMethodSelector selectedMethod={shippingMethod} onChange={onShippingMethodChange} />
      </CheckoutSection>

      <CheckoutSection title="Payment Method">
        <PaymentMethodSelector selectedMethod={paymentMethod} onChange={onPaymentMethodChange} />
        {paymentMethod === "card" ? (
          <div className="checkout-card-fields">
            <CheckoutField label="Card Holder" placeholder="Name on card" />
            <CheckoutField label="Card Number" placeholder="0000 0000 0000 0000" />
            <CheckoutField label="Expiry" placeholder="MM / YY" />
            <CheckoutField label="CVV" placeholder="000" />
          </div>
        ) : null}
      </CheckoutSection>

      <CheckoutSection title="Optional Notes">
        <CheckoutTextarea label="Order Notes" placeholder="Add care requests, delivery preferences, or gift details" />
      </CheckoutSection>
    </form>
  );
}

function CheckoutSection({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section className="checkout-section">
      <h2>{title}</h2>
      {children}
    </section>
  );
}

function CheckoutField({
  label,
  className = "",
  ...inputProps
}: React.InputHTMLAttributes<HTMLInputElement> & { label: string; className?: string }) {
  return (
    <label className={`checkout-field ${className}`}>
      <span>{label}</span>
      <input {...inputProps} />
    </label>
  );
}

function CheckoutSelect({ label }: { label: string }) {
  return (
    <label className="checkout-field checkout-select-field">
      <span>{label}</span>
      <select defaultValue="United States">
        <option>United States</option>
        <option>Canada</option>
        <option>United Kingdom</option>
        <option>France</option>
        <option>Egypt</option>
      </select>
    </label>
  );
}

function CheckoutTextarea({ label, className = "", ...textareaProps }: React.TextareaHTMLAttributes<HTMLTextAreaElement> & { label: string; className?: string }) {
  return (
    <label className={`checkout-field checkout-textarea-field span-two ${className}`}>
      <span>{label}</span>
      <textarea {...textareaProps} />
    </label>
  );
}

export function ShippingMethodSelector({
  selectedMethod,
  onChange,
}: {
  selectedMethod: ShippingMethod;
  onChange: (method: ShippingMethod) => void;
}) {
  return (
    <div className="checkout-option-stack">
      {shippingOptions.map((option) => (
        <button
          key={option.id}
          className={`checkout-option ${selectedMethod === option.id ? "is-selected" : ""}`}
          type="button"
          onClick={() => onChange(option.id)}
        >
          <span>{option.title}</span>
          <strong>{option.displayPrice}</strong>
        </button>
      ))}
    </div>
  );
}

export function PaymentMethodSelector({
  selectedMethod,
  onChange,
}: {
  selectedMethod: PaymentMethod;
  onChange: (method: PaymentMethod) => void;
}) {
  return (
    <div className="checkout-option-stack">
      {paymentOptions.map((option) => (
        <button
          key={option.id}
          className={`checkout-option ${selectedMethod === option.id ? "is-selected" : ""}`}
          type="button"
          onClick={() => onChange(option.id)}
        >
          <span>{option.title}</span>
          <strong>{selectedMethod === option.id ? "Selected" : "Choose"}</strong>
        </button>
      ))}
    </div>
  );
}

type OrderSummaryProps = {
  items: CartItem[];
  totals: {
    subtotal: number;
    shipping: number;
    discount: number;
    tax: number;
    total: number;
  };
  promoCode: string;
  isPromoApplied: boolean;
  onPromoCodeChange: (value: string) => void;
  onApplyPromo: () => void;
  onCompleteOrder: () => void;
};

export function OrderSummary({
  items,
  totals,
  promoCode,
  isPromoApplied,
  onPromoCodeChange,
  onApplyPromo,
  onCompleteOrder,
}: OrderSummaryProps) {
  return (
    <aside className="checkout-summary" aria-label="Order summary">
      <h2>Order Summary</h2>
      <div>
        {items.map((item) => (
          <CheckoutCartItem key={item.product.id} item={item} />
        ))}
      </div>
      <PromoCode
        promoCode={promoCode}
        isPromoApplied={isPromoApplied}
        onPromoCodeChange={onPromoCodeChange}
        onApplyPromo={onApplyPromo}
      />
      <div className="checkout-totals">
        <SummaryRow label="Subtotal" value={formatCurrency(totals.subtotal)} />
        <SummaryRow label="Shipping" value={totals.shipping === 0 ? "Free" : formatCurrency(totals.shipping)} />
        {isPromoApplied ? <SummaryRow label="Promo" value={`- ${formatCurrency(totals.discount)}`} /> : null}
        <SummaryRow label="Tax" value={formatCurrency(totals.tax)} />
        <SummaryRow label="Total" value={formatCurrency(totals.total)} isTotal />
      </div>
      <button className="checkout-complete-button" type="button" onClick={onCompleteOrder}>
        Complete Order
      </button>
    </aside>
  );
}

export function CheckoutCartItem({ item }: { item: CartItem }) {
  const lineTotal = getPriceValue(item.product.price) * item.quantity;

  return (
    <article className="checkout-summary-item">
      <img src={item.product.image} alt={item.product.name} />
      <div>
        <h3>{item.product.name}</h3>
        <p>{formatCurrency(lineTotal)}</p>
      </div>
      <span>{item.quantity}</span>
    </article>
  );
}

function SummaryRow({ label, value, isTotal = false }: { label: string; value: string; isTotal?: boolean }) {
  return (
    <div className={isTotal ? "checkout-total-row is-total" : "checkout-total-row"}>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

export function PromoCode({
  promoCode,
  isPromoApplied,
  onPromoCodeChange,
  onApplyPromo,
}: {
  promoCode: string;
  isPromoApplied: boolean;
  onPromoCodeChange: (value: string) => void;
  onApplyPromo: () => void;
}) {
  return (
    <div className="checkout-promo">
      <input
        value={promoCode}
        onChange={(event) => onPromoCodeChange(event.target.value)}
        placeholder="Promo code"
        aria-label="Promo code"
      />
      <button type="button" onClick={onApplyPromo}>
        Apply
      </button>
      {isPromoApplied ? <p>Private client code applied.</p> : null}
    </div>
  );
}

export function RecommendedProducts({ onProductNavigate }: { onProductNavigate: (product: Product) => void }) {
  return (
    <section className="checkout-recommended" aria-labelledby="checkout-recommended-title">
      <div className="checkout-container">
        <h2 id="checkout-recommended-title">You May Also Like</h2>
        <div className="checkout-recommended-grid">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} onSelect={onProductNavigate} />
          ))}
        </div>
      </div>
    </section>
  );
}

function CheckoutEmptyState({ onShopNavigate }: { onShopNavigate: () => void }) {
  return (
    <section className="checkout-empty-state" aria-label="Empty checkout">
      <h2>Your checkout is empty.</h2>
      <p>Add products to continue.</p>
      <button type="button" onClick={onShopNavigate}>
        Go To Shop
      </button>
    </section>
  );
}

export function CheckoutSuccessState({ onShopNavigate }: { onShopNavigate: () => void }) {
  return (
    <section className="checkout-success-state" aria-label="Order confirmed">
      <h1>Order Confirmed</h1>
      <p>Thank you for choosing SOLVE.</p>
      <div>
        <button type="button" onClick={onShopNavigate}>
          Continue Shopping
        </button>
        <button type="button">
          View Order
        </button>
      </div>
    </section>
  );
}
