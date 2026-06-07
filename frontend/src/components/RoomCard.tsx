import type { Room } from "../types/commerce";

type RoomCardProps = {
  room: Room;
};

export function RoomCard({ room }: RoomCardProps) {
  return (
    <article className="group min-w-[250px] text-center md:min-w-0">
      <div className="relative mx-auto size-[250px] overflow-hidden rounded-full bg-white md:size-[clamp(190px,20vw,300px)]">
        <img
          className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-[1.04]"
          src={room.image}
          alt={`${room.label} room`}
        />
        <div className="absolute inset-0 bg-[rgba(20,16,14,0.24)]" />
        <h3 className="absolute inset-0 grid place-items-center px-8 font-serif text-[38px] font-normal uppercase leading-[0.9] tracking-[-0.055em] text-white md:text-[42px]">
          {room.label}
        </h3>
      </div>
    </article>
  );
}
