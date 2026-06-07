import { rooms } from "../data/landingData";
import { RoomCard } from "./RoomCard";

type RoomsSectionProps = {
  onSeeAll: () => void;
};

export function RoomsSection({ onSeeAll }: RoomsSectionProps) {
  return (
    <section id="rooms" className="bg-paper py-20 md:min-h-[85vh] md:py-[110px]">
      <div className="container-page text-center">
        <h2 className="font-serif text-[clamp(72px,8vw,116px)] font-normal leading-[0.9] tracking-[-0.055em] text-ink">
          Rooms
        </h2>
        <button className="eyebrow-link mt-5 text-muted" type="button" onClick={onSeeAll}>
          See All &rarr;
        </button>
        <div className="-mx-5 mt-14 flex justify-start gap-8 overflow-x-auto px-5 pb-4 md:mx-0 md:grid md:grid-cols-3 md:justify-items-center md:gap-[72px] md:overflow-visible md:px-0 md:pb-0">
          {rooms.map((room) => (
            <RoomCard key={room.id} room={room} />
          ))}
        </div>
      </div>
    </section>
  );
}
