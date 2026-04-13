-- Extensions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Tables with no dependencies
CREATE TABLE initiative_category (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name            TEXT NOT NULL UNIQUE,
    icon_name       TEXT
);

CREATE TABLE initiative_preset (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name            TEXT NOT NULL,
    description     TEXT NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "user" (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name      TEXT NOT NULL,
    last_name       TEXT NOT NULL,
    email           TEXT NOT NULL UNIQUE,
    password_hash   TEXT NOT NULL,
    avatar_image_url TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE community (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name            TEXT NOT NULL,
    latitude        DOUBLE PRECISION,
    longitude       DOUBLE PRECISION,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Tables dependent on user and community
CREATE TABLE community_resident (
    user_id         UUID NOT NULL REFERENCES "user"(id),
    community_id    UUID NOT NULL REFERENCES community(id),
    is_manager      BOOL NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now(),
    PRIMARY KEY (user_id, community_id)
);

CREATE TABLE community_join_request (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    community_id    UUID NOT NULL REFERENCES community(id),
    reviewed_by     UUID REFERENCES "user"(id),
    is_accepted     BOOL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Tables dependent on user, community, initiative_category, initiative_preset
CREATE TABLE initiative (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    community_id        UUID NOT NULL REFERENCES community(id),
    created_by          UUID REFERENCES "user"(id),
    name                TEXT NOT NULL,
    description         TEXT NOT NULL,
    category_id         UUID REFERENCES initiative_category(id),
    preset_id           UUID REFERENCES initiative_preset(id),
    is_public           BOOL NOT NULL,
    latitude            DOUBLE PRECISION NOT NULL,
    longitude           DOUBLE PRECISION NOT NULL,
    starts_at           TIMESTAMPTZ NOT NULL,
    estimated_ends_at   TIMESTAMPTZ,
    ended_at            TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Tables dependent on initiative
CREATE TABLE initiative_share (
    initiative_id       UUID NOT NULL REFERENCES initiative(id),
    target_community_id UUID NOT NULL REFERENCES community(id),
    shared_by_user_id   UUID NOT NULL REFERENCES "user"(id),
    shared_at           TIMESTAMPTZ NOT NULL DEFAULT now(),
    PRIMARY KEY (initiative_id, target_community_id)
);

CREATE TABLE initiative_comment (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    initiative_id   UUID NOT NULL REFERENCES initiative(id),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    body            TEXT NOT NULL,
    image_url       TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE initiative_like (
    initiative_id   UUID NOT NULL REFERENCES initiative(id),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now(),
    PRIMARY KEY (initiative_id, user_id)
);

CREATE TABLE initiative_participator (
    initiative_id   UUID NOT NULL REFERENCES initiative(id),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    joined_at       TIMESTAMPTZ NOT NULL DEFAULT now(),
    PRIMARY KEY (initiative_id, user_id)
);

CREATE TABLE eco_point_transaction (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    community_id    UUID NOT NULL REFERENCES community(id),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    initiative_id   UUID REFERENCES initiative(id),
    amount          INTEGER NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Tables dependent on initiative_comment
CREATE TABLE initiative_comment_like (
    comment_id      UUID NOT NULL REFERENCES initiative_comment(id),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now(),
    PRIMARY KEY (comment_id, user_id)
);

-- Tables dependent only on user
CREATE TABLE direct_message (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sender_id       UUID NOT NULL REFERENCES "user"(id),
    recipient_id    UUID NOT NULL REFERENCES "user"(id),
    body            TEXT NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE notification (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES "user"(id),
    type            TEXT NOT NULL,
    ref_id          UUID,
    is_read         BOOL NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Indexes
CREATE INDEX ON community_resident (community_id);
CREATE INDEX ON community_join_request (user_id);
CREATE INDEX ON community_join_request (community_id);
CREATE INDEX ON initiative (community_id);
CREATE INDEX ON initiative (created_by);
CREATE INDEX ON initiative (category_id);
CREATE INDEX ON initiative_share (target_community_id);
CREATE INDEX ON initiative_comment (initiative_id);
CREATE INDEX ON initiative_comment (user_id);
CREATE INDEX ON initiative_like (user_id);
CREATE INDEX ON initiative_participator (user_id);
CREATE INDEX ON eco_point_transaction (community_id);
CREATE INDEX ON eco_point_transaction (user_id);
CREATE INDEX ON eco_point_transaction (initiative_id);
CREATE INDEX ON direct_message (sender_id);
CREATE INDEX ON direct_message (recipient_id);
CREATE INDEX ON notification (user_id);
CREATE INDEX ON notification (is_read);