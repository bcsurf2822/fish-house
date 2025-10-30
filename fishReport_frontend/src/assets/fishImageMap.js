// Mapping of fish names to their image paths
const BASE_URL = "https://fishclientstorage.blob.core.windows.net/fish-images";

export const fishImageMap = {
  // Tuna varieties
  tuna: `${BASE_URL}/tuna.jpg`,
  "bluefin tuna": `${BASE_URL}/tuna.jpg`,
  "yellowfin tuna": `${BASE_URL}/yfintuna.jpg`,
  "albacore tuna": `${BASE_URL}/tuna.jpg`,
  "skipjack tuna": `${BASE_URL}/tuna.jpg`,
  "bigeye tuna": `${BASE_URL}/tuna.jpg`,

  // Salmon varieties
  salmon: `${BASE_URL}/salmon.jpg`,
  "atlantic salmon": `${BASE_URL}/salmon.jpg`,
  "pacific salmon": `${BASE_URL}/salmon.jpg`,
  "king salmon": `${BASE_URL}/salmon.jpg`,
  "sockeye salmon": `${BASE_URL}/salmon.jpg`,

  // Mackerel varieties
  mackerel: `${BASE_URL}/mackerel.jpg`,
  "king mackerel": `${BASE_URL}/mackerel.jpg`,
  "spanish mackerel": `${BASE_URL}/mackerel.jpg`,
  "atlantic mackerel": `${BASE_URL}/mackerel.jpg`,

  // Billfish
  swordfish: `${BASE_URL}/sword.jpg`,
  marlin: `${BASE_URL}/marlin.jpg`,
  sailfish: `${BASE_URL}/sailfish.jpg`,

  // Reef and coastal fish
  "mahi mahi": `${BASE_URL}/mahi.jpg`,
  halibut: `${BASE_URL}/halibut.jpg`,
  "pacific halibut": `${BASE_URL}/halibut.jpg`,
  grouper: `${BASE_URL}/grouper.jpg`,
  groupers: `${BASE_URL}/grouper.jpg`,
  snapper: `${BASE_URL}/snapper.jpg`,
  anchovy: `${BASE_URL}/anchovies.jpg`,
  anchovies: `${BASE_URL}/anchovies.jpg`,
  wrasse: `${BASE_URL}/wrasse.jpg`,
  "humphead wrasse": `${BASE_URL}/wrasse.jpg`,
  trevally: `${BASE_URL}/trevally.jpg`,
  "giant trevally": `${BASE_URL}/trevally.jpg`,
  tarpon: `${BASE_URL}/tarpon.jpg`,
  "flying fish": `${BASE_URL}/flyingfish.jpg`,
  pompano: `${BASE_URL}/pompano.jpg`,
  sturgeon: `${BASE_URL}/sturgeon.jpg`,
};

export const getFishImage = (fishName) => {
  const normalizedName = fishName.toLowerCase().trim();

  if (fishImageMap[normalizedName]) {
    return fishImageMap[normalizedName];
  }

  for (const [key, value] of Object.entries(fishImageMap)) {
    if (normalizedName.includes(key) || key.includes(normalizedName)) {
      return value;
    }
  }

  return `${BASE_URL}/tuna.jpg`;
};
